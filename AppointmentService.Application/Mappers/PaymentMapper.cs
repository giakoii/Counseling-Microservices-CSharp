using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class PaymentMapper
{
    public static PaymentCollection ToReadModel(Payment payment, string userFullName, string userEmail)
    {
        return new PaymentCollection
        {
            Id = payment.PaymentId,
            UserId = payment.Appointment.StudentId,
            UserFullName = userFullName,
            UserEmail = userEmail,
            Amount = payment.Amount,
            Method = payment.PaymentMethodId.ToString(), // You might want to map this to actual method names
            Status = payment.StatusId?.ToString() ?? "Unknown",
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            CreatedBy = payment.CreatedBy,
            IsActive = payment.IsActive,
            UpdatedAt = payment.UpdatedAt,
            UpdatedBy = payment.UpdatedBy
        };
    }
}
