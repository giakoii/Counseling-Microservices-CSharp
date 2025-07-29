using BuildingBlocks.CQRS;

namespace AppointmentService.Application.CounselorSchedules.Queries.SelectCounselorSchedules;

public record SelectCounselorSchedulesQuery : IQuery<SelectCounselorSchedulesResponse>;
