using BuildingBlocks.CQRS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestTicketService.Application.Commands;
using RequestTicketService.Application.Dtos;
using RequestTicketService.Application.Queries;

namespace RequestTicketService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestTicketChatsController : ControllerBase
    {
        private readonly ICommandHandler<CreateRequestTicketChatCommand, Guid> _createChatHandler;
        private readonly ICommandHandler<UpdateRequestTicketChatCommand, bool> _updateChatHandler;
        private readonly ICommandHandler<DeleteRequestTicketChatCommand, bool> _deleteChatHandler;
        private readonly IQueryHandler<
            GetRequestTicketChatQuery,
            RequestTicketChatDto
        > _getChatHandler;
        private readonly IQueryHandler<
            GetRequestTicketChatsQuery,
            IEnumerable<RequestTicketChatDto>
        > _getChatsHandler;

        public RequestTicketChatsController(
            ICommandHandler<CreateRequestTicketChatCommand, Guid> createChatHandler,
            ICommandHandler<UpdateRequestTicketChatCommand, bool> updateChatHandler,
            ICommandHandler<DeleteRequestTicketChatCommand, bool> deleteChatHandler,
            IQueryHandler<GetRequestTicketChatQuery, RequestTicketChatDto> getChatHandler,
            IQueryHandler<
                GetRequestTicketChatsQuery,
                IEnumerable<RequestTicketChatDto>
            > getChatsHandler
        )
        {
            _createChatHandler = createChatHandler;
            _updateChatHandler = updateChatHandler;
            _deleteChatHandler = deleteChatHandler;
            _getChatHandler = getChatHandler;
            _getChatsHandler = getChatsHandler;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateChat(
            [FromBody] CreateRequestTicketChatCommand command
        )
        {
            var chatId = await _createChatHandler.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetChat), new { chatId }, chatId);
        }

        [HttpGet("{chatId:guid}")]
        [ProducesResponseType(typeof(RequestTicketChatDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChat(Guid chatId)
        {
            var query = new GetRequestTicketChatQuery { ChatId = chatId };
            var result = await _getChatHandler.Handle(query, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestTicketChatDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChats([FromQuery] GetRequestTicketChatsQuery query)
        {
            var result = await _getChatsHandler.Handle(query, CancellationToken.None);
            return Ok(result);
        }

        [HttpPut("{chatId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateChat(
            Guid chatId,
            [FromBody] UpdateRequestTicketChatCommand command
        )
        {
            command.ChatId = chatId;
            await _updateChatHandler.Handle(command, CancellationToken.None);
            return NoContent();
        }

        [HttpDelete("{chatId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            var command = new DeleteRequestTicketChatCommand { ChatId = chatId };
            await _deleteChatHandler.Handle(command, CancellationToken.None);
            return NoContent();
        }
    }
}
