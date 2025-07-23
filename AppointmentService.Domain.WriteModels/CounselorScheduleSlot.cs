using System;
using System.Collections.Generic;

namespace AppointmentService.Infrastructure;

public partial class CounselorScheduleSlot
{
    public Guid Id { get; set; }

    public Guid ScheduleDayId { get; set; }

    public short SlotId { get; set; }

    public short Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual CounselorScheduleDay ScheduleDay { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;
}
