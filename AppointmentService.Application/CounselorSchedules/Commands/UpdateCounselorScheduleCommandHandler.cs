using AppointmentService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using Common;
using Shared.Application.Interfaces;

namespace AppointmentService.Application.CounselorSchedules.Commands;

public record UpdateCounselorScheduleCommand() : ICommand<BaseCommandResponse>;

public class UpdateCounselorScheduleCommandHandler : ICommandHandler<UpdateCounselorScheduleCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<CounselorScheduleDetail> _counselorScheduleRepository;

    public UpdateCounselorScheduleCommandHandler(ICommandRepository<CounselorScheduleDetail> counselorScheduleRepository)
    {
        _counselorScheduleRepository = counselorScheduleRepository;
    }

    public Task<BaseCommandResponse> Handle(UpdateCounselorScheduleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}