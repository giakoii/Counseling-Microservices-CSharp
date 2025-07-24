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
using Shared.Application.Repositories;

public class GetRequestTicketsQueryHandler
    : IQueryHandler<GetRequestTicketsQuery, IEnumerable<RequestTicketDto>>
{
    private readonly ISqlReadRepository<RequestTicket> _repository;

    public GetRequestTicketsQueryHandler(ISqlReadRepository<RequestTicket> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<RequestTicketDto>> Handle(
        GetRequestTicketsQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _repository.GetView<RequestTicket>();

        if (request.StudentId.HasValue)
            query = query.Where(t => t.StudentId == request.StudentId);

        if (request.CounselorId.HasValue)
            query = query.Where(t => t.CounselorId == request.CounselorId);

        if (request.StatusId.HasValue)
            query = query.Where(t => t.StatusId == request.StatusId);

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            int skip = (request.Page.Value - 1) * request.PageSize.Value;
            query = query.Skip(skip).Take(request.PageSize.Value);
        }

        var tickets = await query
            .Include(t => t.RequestTicketChats)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new RequestTicketDto
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
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }
}
