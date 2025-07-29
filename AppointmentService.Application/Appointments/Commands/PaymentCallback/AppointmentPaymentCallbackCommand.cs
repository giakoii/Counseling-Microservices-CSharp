using System.Text.Json.Serialization;
using AppointmentService.Application.Appointments.Commands.InsertAppointment;
using BuildingBlocks.CQRS;

namespace AppointmentService.Application.Appointments.Commands.PaymentCallback;

public record AppointmentPaymentCallbackCommand : ICommand<AppointmentInsertResponse>
{
    [JsonIgnore]
    public Guid AppointmentId { get; set; }
    
    public string Code { get; set; } = null!;
    
    public bool Cancel { get; set; }
}