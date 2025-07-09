using AppointmentService.Application.CounselorSchedules.Commands;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using MassTransit;
using MediatR;

namespace AppointmentService.Application.CounselorSchedules.Consumers;

public class InsertCounselorScheduleEventConsumer: IConsumer<InsertCounselorScheduleRequest>
{
    private readonly IMediator _mediator;

    public InsertCounselorScheduleEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<InsertCounselorScheduleRequest> context)
    {
        var evt = context.Message;
            
        var command = new InsertCounselorScheduleCommand(evt.CounselorEmail, evt.CounselorName);
            
        var response = await _mediator.Send(command);
        
        await context.RespondAsync(response);
    }
}