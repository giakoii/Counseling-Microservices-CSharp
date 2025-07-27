using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Application.Commands;
using RequestTicketService.Domain.Models;
using Shared.Application.Repositories;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class CreateRequestTicketChatCommandHandler
        : ICommandHandler<CreateRequestTicketChatCommand, Guid>
    {
        private readonly ICommandRepository<RequestTicketChat> _commandRepository;

        public CreateRequestTicketChatCommandHandler(
            ICommandRepository<RequestTicketChat> commandRepository
        )
        {
            _commandRepository = commandRepository;
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
            await _commandRepository.AddAsync(chat);
            await _commandRepository.SaveChangesAsync(command.UserId.ToString());
            return chat.ChatId;
        }
    }
}
