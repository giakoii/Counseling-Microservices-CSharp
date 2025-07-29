using Common;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorSchedules;

public class SelectCounselorSchedulesResponse : AbstractResponse<List<SelectCounselorSchedulesEntity>>
{
    public override List<SelectCounselorSchedulesEntity> Response { get; set; } = new();
}

public class SelectCounselorSchedulesEntity
{
    public Guid ScheduleId { get; set; }

    public string CounselorEmail { get; set; }

    public string CounselorName { get; set; } = null!;

    public string Day { get; set; }

    public int DayId { get; set; }

    public string Slot { get; set; }

    public int SlotId { get; set; }

    public short? StatusId { get; set; }
}