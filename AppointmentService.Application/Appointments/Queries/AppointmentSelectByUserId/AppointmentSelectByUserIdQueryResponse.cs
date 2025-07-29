using AppointmentService.Application.Appointments.Queries.AppointmentSelects;
using Common;
using Shared.Infrastructure.Helpers;

namespace AppointmentService.Application.Appointments.Queries.AppointmentSelectByUserId;

public class AppointmentSelectByUserIdQueryResponse : AbstractResponse<PagedResult<AppointmentSelectsQueryEntity>>
{
    public override PagedResult<AppointmentSelectsQueryEntity> Response { get; set; } = new();
}