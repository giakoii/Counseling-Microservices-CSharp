using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Domain.Models;
using Shared.Application.Repositories;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class UpdateRequestTicketCommandHandler
        : ICommandHandler<UpdateRequestTicketCommand, bool>
    {
        private readonly ICommandRepository<RequestTicket> _commandRepository;

        public UpdateRequestTicketCommandHandler(
            ICommandRepository<RequestTicket> commandRepository
        )
        {
            _commandRepository = commandRepository;
        }

        public async Task<bool> Handle(
            UpdateRequestTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = _commandRepository
                .Find(t => t.TicketId == request.TicketId)
                .FirstOrDefault();

            if (ticket == null)
                throw new KeyNotFoundException(
                    $"RequestTicket with ID {request.TicketId} not found"
                );

            ticket.Title = request.Title ?? ticket.Title;
            ticket.Description = request.Description ?? ticket.Description;
            ticket.PriorityId = request.PriorityId ?? ticket.PriorityId;
            ticket.Category = request.Category ?? ticket.Category;
            ticket.StatusId = request.StatusId ?? ticket.StatusId;
            ticket.CounselorId = request.CounselorId ?? ticket.CounselorId;
            ticket.UpdatedBy = request.UpdatedBy;
            ticket.UpdatedAt = DateTime.UtcNow;

            _commandRepository.Update(ticket);
            await _commandRepository.SaveChangesAsync(request.UpdatedBy);

            return true;
        }
    }
}
