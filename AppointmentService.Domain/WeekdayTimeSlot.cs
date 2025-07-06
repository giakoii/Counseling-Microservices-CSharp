namespace AppointmentService.Domain;

public partial class WeekdayTimeSlot
{
    public short DayId { get; set; }

    public int SlotId { get; set; }

    public virtual ICollection<CounselorSchedule> CounselorSchedules { get; set; } = new List<CounselorSchedule>();

    public virtual Weekday Day { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;
}
