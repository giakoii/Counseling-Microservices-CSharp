using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.Appointments.Commands;

public record AppointmentUpdateStatusCommand : ICommand<BaseCommandResponse>
{
    [JsonIgnore]
    [Required(ErrorMessage = "Appointment ID is required.")]
    public Guid AppointmentId { get; set; }
    
    [Required(ErrorMessage = "Status is required.")]
    [Range(3, 4, ErrorMessage = "Status must be between 3 and 4.")]
    public short Status { get; set; }
}

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
        var appointment = await _commandRepository.Find(x => x.AppointmentId == request.AppointmentId && x.IsActive).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (appointment == null || appointmentCollection == null)
        {
            response.SetMessage(MessageId.I00000, "Appointment not found.");
            return response;
        }

        // Begin transaction
        await _commandRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update the status of the appointment
            appointment.StatusId = request.Status;
            appointmentCollection.Status = request.Status;

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

