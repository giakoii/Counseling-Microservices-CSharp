using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;

namespace AuthService.Application.Users.Queries.SelectCounselor;

public record SelectCounselorQuery() : IQuery<SelectCounselorInformationResponse>;
