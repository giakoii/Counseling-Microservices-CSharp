using System.Text.Json.Serialization;

namespace AppointmentService.Domain;

public partial class Weekday
{
    public short Id { get; set; }

    public string DayName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<CounselorScheduleDay> CounselorScheduleDays { get; set; } = new List<CounselorScheduleDay>();
}
