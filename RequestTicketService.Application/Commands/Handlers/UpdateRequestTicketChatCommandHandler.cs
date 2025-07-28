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
    public class UpdateRequestTicketChatCommandHandler
        : ICommandHandler<UpdateRequestTicketChatCommand, bool>
    {
        private readonly IDocumentSession _documentSession;

        public UpdateRequestTicketChatCommandHandler(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<bool> Handle(
            UpdateRequestTicketChatCommand command,
            CancellationToken cancellationToken
        )
        {
            var chat = await _documentSession.LoadAsync<RequestTicketChat>(
                command.ChatId,
                cancellationToken
            );
            if (chat == null || !chat.IsActive)
                return false;

            chat.Message = command.Message;
            chat.MessageTypeId = command.MessageTypeId;
            chat.FileUrl = command.FileUrl;
            chat.IsInternal = command.IsInternal;
            chat.UpdatedAt = DateTime.UtcNow;
            chat.UpdatedBy = command.ChatId.ToString();

            await _documentSession.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
