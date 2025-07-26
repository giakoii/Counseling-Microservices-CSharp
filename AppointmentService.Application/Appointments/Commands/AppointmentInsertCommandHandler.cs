using System.ComponentModel.DataAnnotations;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Marten;
using Shared.Application.Interfaces;

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

public class AppointmentInsertCommandHandler : ICommandHandler<AppointmentInsertCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<Appointment> _appointmentRepository;
    private readonly IIdentityService _identityService;

    public AppointmentInsertCommandHandler(ICommandRepository<Appointment> appointmentRepository, IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
    }

    public async Task<BaseCommandResponse> Handle(AppointmentInsertCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };
        
        var currentUser = _identityService.GetCurrentUser();
        
        var scheduleValid = await _appointmentRepository
            .Find(x => x.ScheduleId == request.ScheduleId 
                       && x.IsActive 
                       && x.StatusId == (byte) ConstantEnum.ScheduleStatus.Available)
            .FirstOrDefaultAsync(cancellationToken);
        if (scheduleValid == null)
        {
            response.SetMessage(MessageId.I00000, "Schedule not found or not available.");
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
            scheduleValid.StatusId = (short) ConstantEnum.ScheduleStatus.Pending;
            _appointmentRepository.Update(scheduleValid);
            
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
            };
            
            // Payment information
            
            
            // Session save changes
            _appointmentRepository.Store(AppointmentCollection.FromWriteModel(newAppointment, userInformation), currentUser.Email);
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