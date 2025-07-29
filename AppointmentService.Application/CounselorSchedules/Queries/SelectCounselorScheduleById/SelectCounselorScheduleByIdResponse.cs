using AppointmentService.Domain.Snapshorts;
using Common;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorScheduleById;

public class SelectCounselorScheduleByIdResponse : AbstractResponse<SelectCounselorScheduleByIdEntity>
{
    public override SelectCounselorScheduleByIdEntity Response { get; set; } = null!;
}

public class SelectCounselorScheduleByIdEntity
{
    public Guid ScheduleId { get; set; }
    public Guid CounselorId { get; set; }
    public string CounselorEmail { get; set; } = null!;
    public string CounselorName { get; set; } = null!;
    public int DayId { get; set; }
    public int SlotId { get; set; }
    public short StatusId { get; set; }
    public string Day { get; set; } = null!;
    public string Slot { get; set; } = null!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public UserInformation Counselor { get; set; } = null!;
}
