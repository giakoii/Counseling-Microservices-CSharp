namespace AppointmentService.Domain;

public partial class Appointment
{
    public Guid AppointmentId { get; set; }

    public Guid StudentId { get; set; }

    public Guid CounselorId { get; set; }

    public Guid ScheduleId { get; set; }

    public string Content { get; set; } = null!;

    public short DurationMinutes { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public short? StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
