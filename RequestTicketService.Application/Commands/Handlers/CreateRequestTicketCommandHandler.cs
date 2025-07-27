using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Domain.Models;
using RequestTicketService.Infrastructure.Data.Contexts;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class CreateRequestTicketCommandHandler
        : ICommandHandler<CreateRequestTicketCommand, Guid>
    {
        private readonly RequestTicketServiceContext _context;

        public CreateRequestTicketCommandHandler(RequestTicketServiceContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(
            CreateRequestTicketCommand command,
            CancellationToken cancellationToken
        )
        {
            var ticket = new RequestTicket
            {
                TicketId = Guid.NewGuid(),
                StudentId = command.StudentId,
                Title = command.Title,
                CounselorId = command.CounselorId,
                Description = command.Description,
                PriorityId = command.PriorityId,
                Category = command.Category,
                CreatedBy = command.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = command.CreatedBy,
                UpdatedAt = DateTime.UtcNow,
            };

            await _context.RequestTickets.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.TicketId;
        }
    }
}
