using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestTicketService.Application.Dtos
{
    public class RequestTicketDto
    {
        public Guid TicketId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CounselorId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public short? PriorityId { get; set; }
        public string? Category { get; set; }
        public short? StatusId { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RequestTicketChatDto> Chats { get; set; } =
            new List<RequestTicketChatDto>();
    }
}
