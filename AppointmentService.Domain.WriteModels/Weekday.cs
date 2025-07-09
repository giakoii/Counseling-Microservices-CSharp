using System;
using System.Collections.Generic;

namespace AppointmentService.Infrastructure;

public partial class Weekday
{
    public short Id { get; set; }

    public string DayName { get; set; } = null!;

    public virtual ICollection<CounselorScheduleDay> CounselorScheduleDays { get; set; } = new List<CounselorScheduleDay>();
}
