using AppointmentService.Application.Appointments.Commands;
using Common;

namespace AppointmentService.Application.PaymentServices;

public interface IPaymentService
{
    Task<AppointmentInsertResponse> PaymentAppointment(Guid appointmentId);
}
