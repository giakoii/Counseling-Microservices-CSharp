using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestTicketService.Application.Commands;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Application.Queries;

namespace RequestTicketService.API.Controllers
{
    [ApiController]
    [Route("api/request-tickets")]
    public class RequestTicketsController : ControllerBase
    {
        private readonly ICommandHandler<CreateRequestTicketCommand, Guid> _createTicketHandler;
        private readonly IQueryHandler<GetRequestTicketQuery, RequestTicketDto> _getTicketHandler;
        private readonly ICommandHandler<UpdateRequestTicketCommand, bool> _updateTicketHandler;
        private readonly ICommandHandler<DeleteRequestTicketCommand, bool> _deleteTicketHandler;

        private readonly IQueryHandler<
            GetRequestTicketsQuery,
            IEnumerable<RequestTicketDto>
        > _getTicketsHandler;

        public RequestTicketsController(
            ICommandHandler<CreateRequestTicketCommand, Guid> createTicketHandler,
            IQueryHandler<GetRequestTicketQuery, RequestTicketDto> getTicketHandler,
            IQueryHandler<GetRequestTicketsQuery, IEnumerable<RequestTicketDto>> getTicketsHandler,
            ICommandHandler<UpdateRequestTicketCommand, bool> updateTicketHandler,
            ICommandHandler<DeleteRequestTicketCommand, bool> deleteTicketHandler
        )
        {
            _createTicketHandler = createTicketHandler;
            _getTicketHandler = getTicketHandler;
            _getTicketsHandler = getTicketsHandler;
            _updateTicketHandler = updateTicketHandler;
            _deleteTicketHandler = deleteTicketHandler;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTicket([FromBody] CreateRequestTicketCommand command)
        {
            var ticketId = await _createTicketHandler.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetTicket), new { ticketId }, ticketId);
        }

        [HttpGet("{ticketId:guid}")]
        [ProducesResponseType(typeof(RequestTicketDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicket(Guid ticketId)
        {
            var query = new GetRequestTicketQuery { TicketId = ticketId };
            var result = await _getTicketHandler.Handle(query, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestTicketDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets([FromQuery] GetRequestTicketsQuery query)
        {
            var result = await _getTicketsHandler.Handle(query, CancellationToken.None);
            return Ok(result);
        }

        [HttpPut("{ticketId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTicket(
            Guid ticketId,
            [FromBody] UpdateRequestTicketCommand command
        )
        {
            if (ticketId != command.TicketId)
                return BadRequest("TicketId mismatch");

            await _updateTicketHandler.Handle(command, CancellationToken.None);
            return Ok();
        }

        //[HttpDelete("{Id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> DeleteTicket(
        //    Guid ticketId,
        //    [FromBody] DeleteRequestTicketCommand command
        //)
        //{
        //    if (ticketId != command.TicketId)
        //        return BadRequest("TicketId mismatch");

        //    await _deleteTicketHandler.Handle(command, CancellationToken.None);
        //    return Ok();
        //}
    }
}
