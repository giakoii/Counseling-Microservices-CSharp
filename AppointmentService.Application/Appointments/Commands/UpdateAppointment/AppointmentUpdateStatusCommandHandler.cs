using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Commands.UpdateAppointment;



/// <summary>
/// AppointmentUpdateStatusCommandHandler - Handles the command to update the status of an appointment.
/// </summary>
public class AppointmentUpdateStatusCommandHandler : ICommandHandler<AppointmentUpdateStatusCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<Appointment> _commandRepository;
    private readonly INoSqlQueryRepository<AppointmentCollection> _appointmentRepository;
    private readonly IIdentityService _identityService;
    
    public AppointmentUpdateStatusCommandHandler(ICommandRepository<Appointment> commandRepository, INoSqlQueryRepository<AppointmentCollection> appointmentRepository, IIdentityService identityService)
    {
        _commandRepository = commandRepository;
        _appointmentRepository = appointmentRepository;
        _identityService = identityService;
    }

    public async Task<BaseCommandResponse> Handle(AppointmentUpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };
        
        // Get current user information
        var user = _identityService.GetCurrentUser();

        // Retrieve the appointment
        var appointmentCollection = await _appointmentRepository.FindOneAsync(x => x.Id == request.AppointmentId && x.IsActive);
        var appointment = await _commandRepository.Find(x => x.AppointmentId == request.AppointmentId && x.IsActive, includes: x=>x.Schedule.Slot).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (appointment == null || appointmentCollection == null)
        {
            response.SetMessage(MessageId.I00000, "Appointment not found.");
            return response;
        }
        
        // Check the time of appointment to update
        if (appointment.Schedule.Slot.StartTime > TimeOnly.FromDateTime(DateTime.UtcNow) && appointment.AppointmentDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            response.SetMessage(MessageId.E00000, "Cannot update status for future appointments.");
            return response;
        }

        // Begin transaction
        await _commandRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update the status of the appointment
            appointment.StatusId = (short) ConstantEnum.AppointmentStatus.Completed;
            appointmentCollection.Status = (short) ConstantEnum.AppointmentStatus.Completed;

            // Save changes
            _commandRepository.Update(appointment);
            await _commandRepository.SaveChangesAsync(user.Email);
            
            // Session save changes
            _commandRepository.Store(appointmentCollection, user.Email, true);
            await _commandRepository.SessionSavechanges();
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}

