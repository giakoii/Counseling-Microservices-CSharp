using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Domain.Models;
using Shared.Application.Interfaces;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class DeleteRequestTicketCommandHandler
        : ICommandHandler<DeleteRequestTicketCommand, bool>
    {
        private readonly ICommandRepository<RequestTicket> _commandRepository;

        public DeleteRequestTicketCommandHandler(
            ICommandRepository<RequestTicket> commandRepository
        )
        {
            _commandRepository = commandRepository;
        }

        public async Task<bool> Handle(
            DeleteRequestTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = _commandRepository
                .Find(t => t.TicketId == request.TicketId && t.IsActive, isTracking: true)
                .FirstOrDefault();

            if (ticket == null)
                throw new KeyNotFoundException(
                    $"RequestTicket with ID {request.TicketId} not found"
                );

            ticket.IsActive = false;
            ticket.UpdatedBy = request.UpdatedBy;
            ticket.UpdatedAt = DateTime.UtcNow;

            _commandRepository.Update(ticket);
            await _commandRepository.SaveChangesAsync(request.UpdatedBy);

            return true;
        }
    }
}
