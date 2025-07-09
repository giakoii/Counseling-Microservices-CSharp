using System.Text.Json.Serialization;

namespace AppointmentService.Domain;

public partial class CounselorScheduleDay
{
    public Guid Id { get; set; }

    public string CounselorEmail { get; set; } = null!;

    public int WeekdayId { get; set; }

    public virtual CounselorSchedule CounselorEmailNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<CounselorScheduleSlot> CounselorScheduleSlots { get; set; } = new List<CounselorScheduleSlot>();

    [JsonIgnore]
    public virtual Weekday Weekday { get; set; } = null!;
}
