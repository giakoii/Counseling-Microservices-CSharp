using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class UpdateRequestTicketChatCommand : ICommand<bool>
    {
        public Guid ChatId { get; set; }
        public string Message { get; set; } = null!;
        public short? MessageTypeId { get; set; }
        public string? FileUrl { get; set; }
        public bool? IsInternal { get; set; }
    }
}
