namespace AppointmentService.Domain;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public short PaymentMethodId { get; set; }

    public short? StatusId { get; set; }

    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;
}
