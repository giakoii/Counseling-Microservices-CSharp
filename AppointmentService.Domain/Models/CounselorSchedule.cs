namespace AppointmentService.Domain.Models;

public partial class CounselorSchedule
{
    public Guid Id { get; set; }

    public Guid CounselorId { get; set; }

    public TimeOnly AvailableFrom { get; set; }

    public TimeOnly AvailableTo { get; set; }

    public short Weekday { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public bool IsActive { get; set; }
}
