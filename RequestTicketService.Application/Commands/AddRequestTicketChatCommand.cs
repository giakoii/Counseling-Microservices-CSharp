using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class AddRequestTicketChatCommand : ICommand<Guid>
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public short? MessageTypeId { get; set; }
        public string? FileUrl { get; set; }
        public bool? IsInternal { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
