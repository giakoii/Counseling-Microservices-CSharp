using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Domain.Models;
using Shared.Application.Repositories;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class DeleteRequestTicketChatCommandHandler
        : ICommandHandler<DeleteRequestTicketChatCommand, bool>
    {
        private readonly ICommandRepository<RequestTicketChat> _commandRepository;
        private readonly ISqlReadRepository<RequestTicketChat> _readRepository;

        public DeleteRequestTicketChatCommandHandler(
            ICommandRepository<RequestTicketChat> commandRepository,
            ISqlReadRepository<RequestTicketChat> readRepository
        )
        {
            _commandRepository = commandRepository;
            _readRepository = readRepository;
        }

        public async Task<bool> Handle(
            DeleteRequestTicketChatCommand command,
            CancellationToken cancellationToken
        )
        {
            var chat = await _readRepository
                .GetView<RequestTicketChat>()
                .FirstOrDefaultAsync(c => c.ChatId == command.ChatId, cancellationToken);
            if (chat == null)
                return false;

            chat.IsActive = false;
            chat.UpdatedAt = DateTime.UtcNow;
            chat.UpdatedBy = "System";
            _commandRepository.Update(chat);
            await _commandRepository.SaveChangesAsync(chat.UpdatedBy);
            return true;
        }
    }
}
