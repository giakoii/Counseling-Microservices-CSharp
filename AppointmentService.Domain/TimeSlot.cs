using System;
using System.Collections.Generic;

namespace AppointmentService.Infrastructure;

public partial class TimeSlot
{
    public int SlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<CounselorSchedule> CounselorSchedules { get; set; } = new List<CounselorSchedule>();

    public virtual ICollection<WeekdayTimeSlot> WeekdayTimeSlots { get; set; } = new List<WeekdayTimeSlot>();
}
