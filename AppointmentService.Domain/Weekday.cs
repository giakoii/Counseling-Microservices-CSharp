namespace AppointmentService.Domain;

public partial class Weekday
{
    public short DayId { get; set; }

    public string DayName { get; set; } = null!;

    public virtual ICollection<CounselorSchedule> CounselorSchedules { get; set; } = new List<CounselorSchedule>();

    public virtual ICollection<WeekdayTimeSlot> WeekdayTimeSlots { get; set; } = new List<WeekdayTimeSlot>();
}
