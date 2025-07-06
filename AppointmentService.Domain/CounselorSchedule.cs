namespace AppointmentService.Domain;

public partial class CounselorSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid CounselorId { get; set; }

    public short DayId { get; set; }

    public int SlotId { get; set; }

    public short? StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual Weekday Day { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;

    public virtual WeekdayTimeSlot WeekdayTimeSlot { get; set; } = null!;
}
