namespace AppointmentService.Domain.Models;

public partial class AppointmentFeedback
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
