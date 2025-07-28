using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Marten;
using RequestTicketService.Application.Commands;
using RequestTicketService.Domain.Models;
using Shared.Application.Interfaces;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class CreateRequestTicketChatCommandHandler
        : ICommandHandler<CreateRequestTicketChatCommand, Guid>
    {
        private readonly IDocumentSession _documentSession;

        public CreateRequestTicketChatCommandHandler(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<Guid> Handle(
            CreateRequestTicketChatCommand command,
            CancellationToken cancellationToken
        )
        {
            var chat = new RequestTicketChat
            {
                ChatId = Guid.NewGuid(),
                TicketId = command.TicketId,
                UserId = command.UserId,
                Message = command.Message,
                MessageTypeId = command.MessageTypeId,
                FileUrl = command.FileUrl,
                IsInternal = command.IsInternal,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.UserId.ToString(),
                IsActive = true,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = command.UserId.ToString(),
            };
            _documentSession.Store(chat);
            await _documentSession.SaveChangesAsync(cancellationToken);
            return chat.ChatId;
        }
    }
}
