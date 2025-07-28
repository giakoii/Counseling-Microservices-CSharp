using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class DeleteRequestTicketCommand : ICommand<bool>
    {
        public Guid TicketId { get; set; }
        public string UpdatedBy { get; set; } = null!;
    }
}
