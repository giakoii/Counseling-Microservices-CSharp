using Common;

namespace AppointmentService.Application.Appointments.Commands.InsertAppointment;

public class AppointmentInsertResponse : AbstractResponse<AppointmentInsertEntity>
{
    public override AppointmentInsertEntity Response { get; set; }
    
    public string AppointmentId { get; set; }
    
    public string TransactionId { get; set; }
}

public class AppointmentInsertEntity
{
    public string CheckoutUrl { get; set; }
    
    public string QrCode { get; set; }
}