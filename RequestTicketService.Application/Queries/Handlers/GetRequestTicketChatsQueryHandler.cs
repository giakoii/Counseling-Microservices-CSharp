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
    public class GetRequestTicketChatsQueryHandler
        : IQueryHandler<GetRequestTicketChatsQuery, IEnumerable<RequestTicketChatDto>>
    {
        private readonly ISqlReadRepository<RequestTicketChat> _readRepository;

        public GetRequestTicketChatsQueryHandler(
            ISqlReadRepository<RequestTicketChat> readRepository
        )
        {
            _readRepository = readRepository;
        }

        public async Task<IEnumerable<RequestTicketChatDto>> Handle(
            GetRequestTicketChatsQuery query,
            CancellationToken cancellationToken
        )
        {
            var chatsQuery = _readRepository.GetView<RequestTicketChat>().Where(c => c.IsActive);

            if (query.TicketId.HasValue)
                chatsQuery = chatsQuery.Where(c => c.TicketId == query.TicketId);

            if (query.UserId.HasValue)
                chatsQuery = chatsQuery.Where(c => c.UserId == query.UserId);

            var chats = await chatsQuery.ToListAsync(cancellationToken);

            return chats.Select(chat => new RequestTicketChatDto
            {
                ChatId = chat.ChatId,
                TicketId = chat.TicketId,
                UserId = chat.UserId,
                Message = chat.Message,
                MessageTypeId = chat.MessageTypeId,
                FileUrl = chat.FileUrl,
                IsInternal = chat.IsInternal,
                CreatedAt = chat.CreatedAt,
            });
        }
    }
}
