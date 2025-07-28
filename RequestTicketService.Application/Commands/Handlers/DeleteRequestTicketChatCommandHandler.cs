using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Marten;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Domain.Models;
using Shared.Application.Interfaces;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class DeleteRequestTicketChatCommandHandler
        : ICommandHandler<DeleteRequestTicketChatCommand, bool>
    {
        private readonly IDocumentSession _documentSession;

        public DeleteRequestTicketChatCommandHandler(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<bool> Handle(
            DeleteRequestTicketChatCommand command,
            CancellationToken cancellationToken
        )
        {
            var chat = await _documentSession.LoadAsync<RequestTicketChat>(
                command.ChatId,
                cancellationToken
            );
            if (chat == null || !chat.IsActive)
                return false;

            chat.IsActive = false;
            chat.UpdatedAt = DateTime.UtcNow;
            chat.UpdatedBy = "system";

            await _documentSession.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
