using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands.Handlers
{
    public class AddRequestTicketChatCommandHandler
        : ICommandHandler<AddRequestTicketChatCommand, Guid>
    {
        public async Task<Guid> Handle(
            AddRequestTicketChatCommand request,
            CancellationToken cancellationToken
        )
        {
            // Implementation of the command handler logic
            return Guid.NewGuid(); // Example return value
        }
    }
}
