using BuildingBlocks.CQRS;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorScheduleById;

public class SelectCounselorScheduleByIdQuery : IQuery<SelectCounselorScheduleByIdResponse>
{
    public Guid ScheduleId { get; set; }
}