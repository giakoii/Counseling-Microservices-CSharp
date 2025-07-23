using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using MassTransit;
using MediatR;

namespace AuthService.Application.Users.Consumers;

public class SelectCounselorEventConsumer : IConsumer<SelectCounselorScheduleEvent>
{
    private readonly IMediator _mediator;

    public SelectCounselorEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SelectCounselorScheduleEvent> context)
    {
        var result = await _mediator.Send(new AuthService.Application.Users.Queries.SelectCounselorQuery());
        
        if (result.Success)
        {
            await context.RespondAsync(new BuildingBlocks.Messaging.Events.CounselorScheduleEvents.SelectCounselorInformationResponse
            {
                Success = true,
                Response = result.Response
            });
        }
        else
        {
            await context.RespondAsync(new BuildingBlocks.Messaging.Events.CounselorScheduleEvents.SelectCounselorInformationResponse
            {
                Success = false,
                Message = result.Message
            });
        }
    }
}