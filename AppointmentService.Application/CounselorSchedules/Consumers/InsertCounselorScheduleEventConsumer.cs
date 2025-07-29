using AppointmentService.Application.CounselorSchedules.Commands;
using AppointmentService.Application.CounselorSchedules.Commands.InsertCounselorSchedule;
using AppointmentService.Domain.Snapshorts;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using MassTransit;
using MediatR;

namespace AppointmentService.Application.CounselorSchedules.Consumers;

public class InsertCounselorScheduleEventConsumer: IConsumer<UserInformationRequest>
{
    private readonly IMediator _mediator;

    public InsertCounselorScheduleEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserInformationRequest> context)
    {
        var evt = context.Message;

        var userInf = new UserInformation
        {
            Id = evt.CounselorId,
            Email = evt.Email,
            FirstName = evt.FirstName,
            LastName = evt.LastName,
        };
            
        var command = new InsertCounselorScheduleCommand(userInf);
            
        var response = await _mediator.Send(command);
        
        await context.RespondAsync(response);
    }
}