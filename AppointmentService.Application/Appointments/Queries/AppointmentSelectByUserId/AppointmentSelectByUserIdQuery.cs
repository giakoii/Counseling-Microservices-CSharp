using BuildingBlocks.CQRS;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelectByUserId;

public class AppointmentSelectByUserIdQuery : IQuery<AppointmentSelectByUserIdQueryResponse>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}