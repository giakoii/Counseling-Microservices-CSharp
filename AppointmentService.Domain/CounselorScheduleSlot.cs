using System.Text.Json.Serialization;

namespace AppointmentService.Domain;

public partial class CounselorScheduleSlot
{
    public Guid Id { get; set; }

    public Guid ScheduleDayId { get; set; }

    public short SlotId { get; set; }

    [JsonIgnore]
    public virtual CounselorScheduleDay ScheduleDay { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;
}
