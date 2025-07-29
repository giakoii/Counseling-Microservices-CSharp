using BuildingBlocks.CQRS;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorSchedulesByCounselorId;

public class SelectCounselorSchedulesByCounselorIdQuery : IQuery<SelectCounselorSchedulesByCounselorIdResponse>
{
    public Guid CounselorId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}