using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestTicketService.Application.Dtos
{
    public class RequestTicketChatDto
    {
        public Guid ChatId { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;
        public short? MessageTypeId { get; set; }
        public string? FileUrl { get; set; }
        public bool? IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
