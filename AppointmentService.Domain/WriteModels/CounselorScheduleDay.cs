namespace AppointmentService.Domain.WriteModels;

public partial class CounselorScheduleDay
{
    public Guid Id { get; set; }

    public string CounselorEmail { get; set; } = null!;

    public short WeekdayId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual CounselorSchedule CounselorEmailNavigation { get; set; } = null!;

    public virtual ICollection<CounselorScheduleSlot> CounselorScheduleSlots { get; set; } = new List<CounselorScheduleSlot>();

    public virtual Weekday Weekday { get; set; } = null!;
}
