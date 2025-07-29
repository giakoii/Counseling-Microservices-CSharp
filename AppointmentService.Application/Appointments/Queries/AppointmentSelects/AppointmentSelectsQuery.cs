using BuildingBlocks.CQRS;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelects;

public record AppointmentSelectsQuery : IQuery<AppointmentSelectsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}