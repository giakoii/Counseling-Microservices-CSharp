using AppointmentService.Domain.WriteModels;
using AppointmentService.Infrastructure;
using BuildingBlocks.CQRS;
using Common;
using Shared.Application.Repositories;

namespace AppointmentService.Application.Appointments;

public record AppointmentInsertCommand() : ICommand<BaseCommandResponse>;

public class AppointmentInsertCommandHandler : ICommandHandler<AppointmentInsertCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<Appointment> _appointmentRepository;

    public AppointmentInsertCommandHandler(ICommandRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public Task<BaseCommandResponse> Handle(AppointmentInsertCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse { Success = false };

        return null;
    }
}