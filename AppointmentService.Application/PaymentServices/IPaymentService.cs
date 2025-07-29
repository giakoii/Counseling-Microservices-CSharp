using AppointmentService.Application.Appointments.Commands;
using AppointmentService.Application.Appointments.Commands.InsertAppointment;
using Common;

namespace AppointmentService.Application.PaymentServices;

public interface IPaymentService
{
    Task<AppointmentInsertResponse> PaymentAppointment(Guid appointmentId);
}
