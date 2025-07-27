using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;

namespace RequestTicketService.Application.Commands
{
    public class CreateRequestTicketCommand : ICommand<Guid>
    {
        public Guid StudentId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public short? PriorityId { get; set; }
        public string? Category { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
