using BuildingBlocks.CQRS;

namespace AppointmentService.Application.Appointments.Queries.SelectAppointmentById;

public class AppointmentSelectByIdQuery : IQuery<AppointmentSelectByIdQueryResponse>
{
    public Guid AppointmentId { get; set; }
}