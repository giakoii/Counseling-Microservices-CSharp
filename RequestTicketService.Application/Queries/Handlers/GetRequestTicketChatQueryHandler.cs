using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Marten;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Domain.Models;
using Shared.Application.Interfaces;

namespace RequestTicketService.Application.Queries.Handlers
{
    public class GetRequestTicketChatQueryHandler
        : IQueryHandler<GetRequestTicketChatQuery, RequestTicketChatDto>
    {
        private readonly IQuerySession _querySession;

        public GetRequestTicketChatQueryHandler(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        public async Task<RequestTicketChatDto> Handle(
            GetRequestTicketChatQuery query,
            CancellationToken cancellationToken
        )
        {
            var chat = await _querySession.LoadAsync<RequestTicketChat>(
                query.ChatId,
                cancellationToken
            );
            if (chat == null || !chat.IsActive)
                throw new Exception("Chat not found");

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
