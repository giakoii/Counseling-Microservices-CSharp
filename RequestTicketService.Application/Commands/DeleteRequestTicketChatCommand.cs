using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class DeleteRequestTicketChatCommand : ICommand<bool>
    {
        public Guid ChatId { get; set; }
    }
}
