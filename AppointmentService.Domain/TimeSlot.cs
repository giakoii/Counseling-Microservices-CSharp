using System.Text.Json.Serialization;

namespace AppointmentService.Domain;

public partial class TimeSlot
{
    public short Id { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [JsonIgnore]
    public virtual ICollection<CounselorScheduleSlot> CounselorScheduleSlots { get; set; } = new List<CounselorScheduleSlot>();
}
