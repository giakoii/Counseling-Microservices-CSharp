using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class UpdateRequestTicketCommand : ICommand<bool>
    {
        public Guid TicketId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public short? PriorityId { get; set; }
        public string? Category { get; set; }
        public short? StatusId { get; set; }
        public Guid? CounselorId { get; set; }
        public string UpdatedBy { get; set; } = null!;
    }
}
