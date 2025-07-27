using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Application.Queries;
using RequestTicketService.Domain.Models;
using Shared.Application.Interfaces;

namespace RequestTicketService.Application.Queries.Handlers
{
    public class RequestTicketQueryHandler
        : IQueryHandler<GetRequestTicketIdQuery, RequestTicketDto>,
            IQueryHandler<GetRequestTicketsQuery, IEnumerable<RequestTicketDto>>
    {
        private readonly ISqlReadRepository<RequestTicket> _repository;

        public RequestTicketQueryHandler(ISqlReadRepository<RequestTicket> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<RequestTicketDto> Handle(
            GetRequestTicketIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var entity = await BaseEntityQuery()
                .FirstOrDefaultAsync(t => t.TicketId == request.TicketId, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException(
                    $"RequestTicket with ID {request.TicketId} not found"
                );

            return MapToDto(entity);
        }

        public async Task<IEnumerable<RequestTicketDto>> Handle(
            GetRequestTicketsQuery request,
            CancellationToken cancellationToken
        )
        {
            var query = BaseEntityQuery();

            if (request.StudentId.HasValue)
                query = query.Where(t => t.StudentId == request.StudentId.Value);
            if (request.CounselorId.HasValue)
                query = query.Where(t => t.CounselorId == request.CounselorId.Value);
            if (request.StatusId.HasValue)
                query = query.Where(t => t.StatusId == request.StatusId.Value);

            if (request.Page.HasValue && request.PageSize.HasValue)
            {
                int skip = (request.Page.Value - 1) * request.PageSize.Value;
                query = query.Skip(skip).Take(request.PageSize.Value);
            }

            var entities = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToDto);
        }

        private IQueryable<RequestTicket> BaseEntityQuery()
        {
            return _repository.GetView<RequestTicket>().Include(t => t.RequestTicketChats);
        }

        private RequestTicketDto MapToDto(RequestTicket t)
        {
            return new RequestTicketDto
            {
                TicketId = t.TicketId,
                StudentId = t.StudentId,
                CounselorId = t.CounselorId,
                Title = t.Title,
                Description = t.Description,
                PriorityId = t.PriorityId,
                Category = t.Category,
                StatusId = t.StatusId,
                ResolvedAt = t.ResolvedAt,
                ClosedAt = t.ClosedAt,
                CreatedAt = t.CreatedAt,
                IsActive = t.IsActive,
                Chats = t
                    .RequestTicketChats.Select(c => new RequestTicketChatDto
                    {
                        ChatId = c.ChatId,
                        TicketId = c.TicketId,
                        UserId = c.UserId,
                        Message = c.Message,
                        MessageTypeId = c.MessageTypeId,
                        FileUrl = c.FileUrl,
                        IsInternal = c.IsInternal,
                        CreatedAt = c.CreatedAt,
                    })
                    .ToList(),
            };
        }
    }
}
