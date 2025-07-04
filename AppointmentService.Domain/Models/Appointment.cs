namespace AppointmentService.Domain.Models;

public partial class Appointment
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid CounselorId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public short Status { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<AppointmentFeedback> AppointmentFeedbacks { get; set; } = new List<AppointmentFeedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
