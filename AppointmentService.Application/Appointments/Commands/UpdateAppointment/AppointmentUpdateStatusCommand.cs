using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BuildingBlocks.CQRS;
using Common;

namespace AppointmentService.Application.Appointments.Commands.UpdateAppointment;

public record AppointmentUpdateStatusCommand : ICommand<BaseCommandResponse>
{
    [JsonIgnore]
    [Required(ErrorMessage = "Appointment ID is required.")]
    public Guid AppointmentId { get; set; }
}