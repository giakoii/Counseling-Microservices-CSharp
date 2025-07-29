using AppointmentService.Application.PaymentServices;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Commands.InsertAppointment;


/// <summary>
/// AppointmentInsertCommandHandler - Handles the insertion of a new appointment.
/// </summary>
public class AppointmentInsertCommandHandler : ICommandHandler<AppointmentInsertCommand, AppointmentInsertResponse>
{
    private readonly ICommandRepository<Appointment> _appointmentRepository;
    private readonly ICommandRepository<CounselorScheduleDetail> _counselorScheduleRepository;
    private readonly ICommandRepository<Payment> _paymentRepository;
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleDetailRepository;
    private readonly IIdentityService _identityService;
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    /// <param name="identityService"></param>
    /// <param name="counselorScheduleRepository"></param>
    /// <param name="counselorScheduleDetailRepository"></param>
    /// <param name="paymentService"></param>
    /// <param name="paymentRepository"></param>
    public AppointmentInsertCommandHandler(ICommandRepository<Appointment> appointmentRepository, IIdentityService identityService, ICommandRepository<CounselorScheduleDetail> counselorScheduleRepository, INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleDetailRepository, IPaymentService paymentService, ICommandRepository<Payment> paymentRepository)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
        _counselorScheduleRepository = counselorScheduleRepository;
        _counselorScheduleDetailRepository = counselorScheduleDetailRepository;
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
    }

    public async Task<AppointmentInsertResponse> Handle(AppointmentInsertCommand request, CancellationToken cancellationToken)
    {
        var response = new AppointmentInsertResponse { Success = false };
        
        var currentUser = _identityService.GetCurrentUser();
        
        var scheduleValid = await _counselorScheduleRepository
            .Find(x => x.Id == request.ScheduleId 
                       && x.IsActive 
                       && x.Status == ((byte) ConstantEnum.ScheduleStatus.Available),
               isTracking: true,
               x => x.Slot, x => x.Weekday).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (scheduleValid == null)
        {
            response.SetMessage(MessageId.I00000, "Schedule not found or not available.");
            return response;
        }
        
        var counselorInf = await _counselorScheduleDetailRepository.FindOneAsync(x => x.Id == scheduleValid.Id);
        if (counselorInf == null)
        {
            response.SetMessage(MessageId.I00000, "Counselor information not found.");
            return response;
        }
        
        // Get current date and calculate the next appointment date based on the counselor's weekday
        var today = DateTime.UtcNow.Date;
        int targetWeekday = counselorInf.WeekdayId;
        
        int daysUntil = ((targetWeekday - (int)today.DayOfWeek + 7) % 7);
        if (daysUntil == 0) daysUntil = 7;
        
        var appointmentDate = today.AddDays(daysUntil);
        
        // Begin transaction
        await _appointmentRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new appointment
            var newAppointment = new Appointment
            {
                StudentId = Guid.Parse(currentUser.UserId),
                ScheduleId = request.ScheduleId,
                Content = request.Content,
                AppointmentDate = DateOnly.FromDateTime(appointmentDate),
                StatusId = (short) ConstantEnum.AppointmentStatus.Pending,
            };
            
            // Update schedule status
            scheduleValid.Status = (short) ConstantEnum.ScheduleStatus.Pending;
            counselorInf.StatusId = (short) ConstantEnum.ScheduleStatus.Pending;
            
            _counselorScheduleRepository.Update(scheduleValid);
            
            // Save changes
            await _appointmentRepository.AddAsync(newAppointment);
            await _appointmentRepository.SaveChangesAsync(currentUser.Email);

            
            // Create user information
            var names = currentUser.FullName?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var firstName = names.Length > 0 ? names[0] : string.Empty;
            var lastName = names.Length > 1 ? names[^1] : string.Empty;

            var userInformation = new UserInformation
            {
                FirstName = firstName,
                Email = currentUser.Email,
                LastName = lastName,
                Id = Guid.Parse(currentUser.UserId),
            };
            
            // Payment information
            var paymentResponse = await _paymentService.PaymentAppointment(newAppointment.AppointmentId);
            var newPayment = new Payment
            {
                AppointmentId = newAppointment.AppointmentId,
                Amount = 2000,
                StatusId = ((short)ConstantEnum.PaymentStatus.Pending),
                PaymentMethodId = ((short)ConstantEnum.PaymentMethod.PayOs),
                TransactionId = paymentResponse.TransactionId,
            };
            
            await _paymentRepository.AddAsync(newPayment);
            await _paymentRepository.SaveChangesAsync(currentUser.Email);
            
            var appointmentCollection = AppointmentCollection.FromWriteModel(newAppointment, counselorInf.Counselor, userInformation, true);
            
            // Session save changes
            _appointmentRepository.Store(appointmentCollection, currentUser.Email);
            _counselorScheduleRepository.Store(counselorInf, currentUser.Email, true);
            _paymentRepository.Store(PaymentCollection.FromWriteModel(newPayment, appointmentCollection), currentUser.Email);
            await _appointmentRepository.SessionSavechanges();

            // True
            response.Success = true;
            response.AppointmentId = newAppointment.AppointmentId.ToString();
            response.Response = paymentResponse.Response;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}