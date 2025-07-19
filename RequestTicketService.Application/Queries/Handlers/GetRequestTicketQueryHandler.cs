using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Application.Dtos;

namespace RequestTicketService.Application.Queries.Handlers
{
    public class GetRequestTicketQueryHandler
        : IQueryHandler<GetRequestTicketQuery, RequestTicketDto>
    {
        public async Task<RequestTicketDto> Handle(
            GetRequestTicketQuery request,
            CancellationToken cancellationToken
        )
        {
            // Add logic to retrieve the RequestTicketDto based on the request.TicketId
            // For now, returning a placeholder object
            return await Task.FromResult(
                new RequestTicketDto
                {
                    TicketId = request.TicketId,
                    Title = "Sample Ticket",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                }
            );
        }
    }
}
