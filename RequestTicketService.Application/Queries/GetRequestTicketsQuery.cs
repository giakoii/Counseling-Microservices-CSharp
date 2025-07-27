using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using RequestTicketService.Application.Dtos;

namespace RequestTicketService.Application.Queries
{
    public class GetRequestTicketsQuery : IQuery<IEnumerable<RequestTicketDto>>
    {
        public Guid? StudentId { get; set; }
        public Guid? CounselorId { get; set; }
        public short? StatusId { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public Guid TicketId { get; set; }
    }
}
