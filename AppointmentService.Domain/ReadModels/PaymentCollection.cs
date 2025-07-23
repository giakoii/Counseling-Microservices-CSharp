using System.Text.Json.Serialization;
using AppointmentService.Domain.Snapshorts;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Domain.ReadModels;

public class PaymentCollection
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("method")]
    public int Method { get; set; }

    [JsonPropertyName("status")]
    public short Status { get; set; }

    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = null!;

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("updated_by")]
    public string UpdatedBy { get; set; } = null!;
    
    [JsonPropertyName("appointment_id")]
    public Guid AppointmentId { get; set; }
    
    public AppointmentCollection Appointment { get; set; } = null!;
    public UserInformation User { get; set; } = null!;

    public static PaymentCollection FromWriteModel(Payment model, UserInformation user, bool includeAppointment = false)
    {
        var collection = new PaymentCollection
        {
            Id = model.PaymentId,
            Amount = model.Amount,
            Method = model.PaymentMethodId,
            Status = model.StatusId ?? 1,
            TransactionId = model.TransactionId,
            CreatedAt = model.CreatedAt,
            CreatedBy = model.CreatedBy,
            IsActive = model.IsActive,
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = model.UpdatedBy,
            UserId = model.Appointment.StudentId,
            AppointmentId = model.AppointmentId,
            Appointment = AppointmentCollection.FromWriteModel(model.Appointment, user)
        };

        if (includeAppointment)
        {
            collection.User = user;
        }

        return collection;
    }
}