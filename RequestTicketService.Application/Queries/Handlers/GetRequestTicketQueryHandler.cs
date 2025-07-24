using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Domain.Models;
using Shared.Application.Repositories;

namespace RequestTicketService.Application.Queries.Handlers
{
    public class GetRequestTicketQueryHandler
        : IQueryHandler<GetRequestTicketQuery, RequestTicketDto>
    {
        private readonly ISqlReadRepository<RequestTicket> _readRepository;

        public GetRequestTicketQueryHandler(ISqlReadRepository<RequestTicket> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<RequestTicketDto> Handle(
            GetRequestTicketQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _readRepository
                .GetView<RequestTicket>()
                .FirstOrDefaultAsync(t => t.TicketId == request.TicketId, cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException(
                    $"RequestTicket with ID {request.TicketId} not found"
                );

            return new RequestTicketDto
            {
                TicketId = ticket.TicketId,
                StudentId = ticket.StudentId,
                CounselorId = ticket.CounselorId,
                Title = ticket.Title,
                Description = ticket.Description,
                PriorityId = ticket.PriorityId,
                Category = ticket.Category,
                StatusId = ticket.StatusId,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                CreatedAt = ticket.CreatedAt,
                IsActive = ticket.IsActive,
            };
        }
    }
}
