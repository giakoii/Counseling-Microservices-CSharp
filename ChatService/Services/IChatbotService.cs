using ChatService.Models;

namespace ChatService.Services
{
    public interface IChatbotService
    {
        Task<ChatResponse> AskAsync(string chatRequest);
    }
}