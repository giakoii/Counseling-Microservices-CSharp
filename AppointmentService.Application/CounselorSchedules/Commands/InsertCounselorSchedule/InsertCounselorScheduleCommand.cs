using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.CQRS;
using Common;

namespace AppointmentService.Application.CounselorSchedules.Commands.InsertCounselorSchedule;

public record InsertCounselorScheduleCommand(UserInformation Counselor) : ICommand<BaseCommandResponse>;