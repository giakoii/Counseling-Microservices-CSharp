namespace AppointmentService.Domain.WriteModels;

public partial class CounselorSchedule
{
    public string CounselorEmail { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<CounselorScheduleDay> CounselorScheduleDays { get; set; } = new List<CounselorScheduleDay>();
}
