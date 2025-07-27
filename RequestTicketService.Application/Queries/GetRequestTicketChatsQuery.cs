using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Application.Dtos;

namespace RequestTicketService.Application.Queries
{
    public class GetRequestTicketChatsQuery : IQuery<IEnumerable<RequestTicketChatDto>>
    {
        public Guid? TicketId { get; set; }
        public Guid? UserId { get; set; }
    }
}
