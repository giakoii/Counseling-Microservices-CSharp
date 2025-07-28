using BuildingBlocks.CQRS;
using Marten;
using RequestTicketService.Application.Commands;
using RequestTicketService.Domain.Models;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class CreateRequestTicketCommandHandler
        : ICommandHandler<CreateRequestTicketCommand, Guid>
    {
        private readonly Marten.IDocumentSession _documentSession;

        public CreateRequestTicketCommandHandler(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<Guid> Handle(
            CreateRequestTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = new RequestTicket
            {
                TicketId = Guid.NewGuid(),
                StudentId = request.StudentId,
                Title = request.Title,
                Description = request.Description,
                PriorityId = request.PriorityId,
                Category = request.Category,
                StatusId = 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "system",
                UpdatedBy = "system",
            };

            _documentSession.Store(ticket);
            await _documentSession.SaveChangesAsync();

            return ticket.TicketId;
        }
    }
}
