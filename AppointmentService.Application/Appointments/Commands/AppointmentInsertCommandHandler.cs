using System.ComponentModel.DataAnnotations;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Shared.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Application.Appointments.Commands;

public class AppointmentInsertCommand : ICommand<BaseCommandResponse>
{
    [Required(ErrorMessage = "ScheduleId is required.")]
    public Guid ScheduleId { get; set; }
    
    [Required(ErrorMessage = "Content is required.")]
    [MaxLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
    public string Content { get; set; } = null!;
    
    [Required(ErrorMessage = "AppointmentDate is required.")]
    public DateOnly AppointmentDate { get; set; }
} 

/// <summary>
/// AppointmentInsertCommandHandler - Handles the insertion of a new appointment.
/// </summary>
public class AppointmentInsertCommandHandler : ICommandHandler<AppointmentInsertCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<Appointment> _appointmentRepository;
    private readonly ICommandRepository<CounselorScheduleDetail> _counselorScheduleRepository;
    private readonly INoSqlQueryRepository<CounselorScheduleDetailCollection> _counselorScheduleDetailRepository;
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appointmentRepository"></param>
    /// <param name="identityService"></param>
    /// <param name="counselorScheduleRepository"></param>
    /// <param name="counselorScheduleDetailRepository"></param>
    public AppointmentInsertCommandHandler(ICommandRepository<Appointment> appointmentRepository, IIdentityService identityService, ICommandRepository<CounselorScheduleDetail> counselorScheduleRepository, INoSqlQueryRepository<CounselorScheduleDetailCollection> counselorScheduleDetailRepository)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
        _counselorScheduleRepository = counselorScheduleRepository;
        _counselorScheduleDetailRepository = counselorScheduleDetailRepository;
    }

    public async Task<BaseCommandResponse> Handle(AppointmentInsertCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };
        
        var currentUser = _identityService.GetCurrentUser();
        
        var scheduleValid = await _counselorScheduleRepository
            .Find(x => x.Id == request.ScheduleId 
                       && x.IsActive 
                       && x.Status == ((byte) ConstantEnum.ScheduleStatus.Available),
               isTracking: true,
               x => x.Slot, x => x.Weekday).FirstOrDefaultAsync(cancellationToken);
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
        
        // Begin transaction
        await _appointmentRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new appointment
            var newAppointment = new Appointment
            {
                StudentId = Guid.Parse(currentUser.UserId),
                ScheduleId = request.ScheduleId,
                Content = request.Content,
                AppointmentDate = request.AppointmentDate,
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
            
            
            // Session save changes
            _appointmentRepository.Store(AppointmentCollection.FromWriteModel(newAppointment, counselorInf.Counselor, userInformation), currentUser.Email);
            _counselorScheduleRepository.Store(counselorInf, currentUser.Email, true);
            await _appointmentRepository.SessionSavechanges();

            // True
            response.Success = true;
            response.Response = newAppointment.AppointmentId.ToString();
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}