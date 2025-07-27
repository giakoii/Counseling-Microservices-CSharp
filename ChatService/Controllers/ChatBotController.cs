using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ChatService.Services;
using ChatService.Models;

namespace ChatService.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatBotController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatBotController> _logger;

        private readonly ChatbotService _chatbot;

        public ChatBotController(IWebHostEnvironment env, ILogger<ChatBotController> logger, ChatbotService chatbot)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _logger = logger;
            _chatbot = chatbot;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Please provide a question.");

            var answer = await _chatbot.AskAsync(request.Question);
            return Ok(new ChatResponse { Answer = answer.Answer, TextTime = answer.TextTime });
        }

    }
}
