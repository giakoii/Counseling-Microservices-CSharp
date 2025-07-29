using AppointmentService.Application.Appointments.Queries.AppointmentSelects;
using Common;

namespace AppointmentService.Application.Appointments.Queries.SelectAppointmentById;

public class AppointmentSelectByIdQueryResponse : AbstractResponse<AppointmentSelectsQueryEntity>
{
    public override AppointmentSelectsQueryEntity Response { get; set; } = null!;
}