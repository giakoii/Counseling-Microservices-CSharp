using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Application.Queries;

public class GetRequestTicketsQueryHandler
    : IQueryHandler<GetRequestTicketsQuery, IEnumerable<RequestTicketDto>>
{
    public Task<IEnumerable<RequestTicketDto>> Handle(
        GetRequestTicketsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Implementation for handling the query
        throw new NotImplementedException();
    }
}
