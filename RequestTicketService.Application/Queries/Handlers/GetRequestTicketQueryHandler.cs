using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
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
        private readonly INoSqlQueryRepository<RequestTicket> _ticketRepository;
        private readonly INoSqlQueryRepository<RequestTicketChat> _chatRepository;

        public RequestTicketQueryHandler(
            INoSqlQueryRepository<RequestTicket> ticketRepository,
            INoSqlQueryRepository<RequestTicketChat> chatRepository
        )
        {
            _ticketRepository = ticketRepository;
            _chatRepository = chatRepository;
        }

        public async Task<RequestTicketDto> Handle(
            GetRequestTicketIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _ticketRepository.FindOneAsync(t => t.TicketId == request.TicketId);

            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

            var chats = await _chatRepository.FindAllAsync(c => c.TicketId == request.TicketId);
            ticket.RequestTicketChats = chats;

            return MapToDto(ticket);
        }

        public async Task<IEnumerable<RequestTicketDto>> Handle(
            GetRequestTicketsQuery request,
            CancellationToken cancellationToken
        )
        {
            var allTickets = await _ticketRepository.FindAllAsync(t => t.IsActive);
            var query = allTickets.AsQueryable();

            if (request.StudentId.HasValue)
                query = query.Where(t => t.StudentId == request.StudentId.Value);

            if (request.CounselorId.HasValue)
                query = query.Where(t => t.CounselorId == request.CounselorId.Value);

            if (request.StatusId.HasValue)
                query = query.Where(t => t.StatusId == request.StatusId.Value);

            if (request.Page.HasValue && request.PageSize.HasValue)
                query = query
                    .Skip((request.Page.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);

            var tickets = query.OrderByDescending(t => t.CreatedAt).ToList();

            foreach (var ticket in tickets)
            {
                var chats = await _chatRepository.FindAllAsync(c => c.TicketId == ticket.TicketId);
                ticket.RequestTicketChats = chats;
            }

            return tickets.Select(MapToDto);
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
                Chats =
                    t.RequestTicketChats?.Select(c => new RequestTicketChatDto
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
                        .ToList() ?? new(),
            };
        }
    }
}
