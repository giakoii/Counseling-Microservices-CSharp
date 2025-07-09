namespace AppointmentService.Domain.WriteModels;

public partial class TimeSlot
{
    public short Id { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<CounselorScheduleSlot> CounselorScheduleSlots { get; set; } = new List<CounselorScheduleSlot>();
}
