using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Domain.Models;
using Shared.Application.Repositories;

namespace RequestTicketService.Application.Queries.Handlers
{
    public class GetRequestTicketChatQueryHandler
        : IQueryHandler<GetRequestTicketChatQuery, RequestTicketChatDto>
    {
        private readonly ISqlReadRepository<RequestTicketChat> _readRepository;

        public GetRequestTicketChatQueryHandler(
            ISqlReadRepository<RequestTicketChat> readRepository
        )
        {
            _readRepository = readRepository;
        }

        public async Task<RequestTicketChatDto> Handle(
            GetRequestTicketChatQuery query,
            CancellationToken cancellationToken
        )
        {
            var chat = await _readRepository
                .GetView<RequestTicketChat>()
                .FirstOrDefaultAsync(c => c.ChatId == query.ChatId, cancellationToken);

            return new RequestTicketChatDto
            {
                ChatId = chat.ChatId,
                TicketId = chat.TicketId,
                UserId = chat.UserId,
                Message = chat.Message,
                MessageTypeId = chat.MessageTypeId,
                FileUrl = chat.FileUrl,
                IsInternal = chat.IsInternal,
                CreatedAt = chat.CreatedAt,
            };
        }
    }
}
