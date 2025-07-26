using ChatService.Services;
using ChatService.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatService.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<ChatbotService> _logger;

        public ChatbotService(HttpClient httpClient, IConfiguration config, ILogger<ChatbotService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<ChatResponse> AskAsync(string chatRequest)
        {
            try
            {
                string context = ContextLoader.LoadCombinedContext();
                string apiKey = _config["Gemini:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("Gemini API key is not configured.");
                }

                // Combine system prompt with context and user question for Gemini
                string fullPrompt = $@"You are an admission consultant at FPT University. Answer based only on the provided context. Keep responses concise.

Context:
{context}

User Question: {chatRequest}

Please provide a helpful answer based only on the context provided above.";

                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = fullPrompt }
                        }
                    }
                },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        maxOutputTokens = 200,
                        topP = 0.8,
                        topK = 10
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gemini API endpoint with API key as query parameter
                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Gemini API Response Status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API returned error. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseJson);

                    // Handle specific quota error
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        return new ChatResponse
                        {
                            Answer = "I'm currently experiencing high demand. Please check your Gemini API quota and try again later.",
                            TextTime = DateTime.Now
                        };
                    }

                    return new ChatResponse
                    {
                        Answer = "I'm temporarily unavailable due to a service issue. Please try again later.",
                        TextTime = DateTime.Now
                    };
                }

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                // Check if the response has an error
                if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Unknown API error";
                    _logger.LogError("Gemini API error: {Error}", errorMessage);

                    string answer = errorMessage.Contains("quota") || errorMessage.Contains("limit")
                        ? "I'm currently unavailable due to usage limits. Please check your Gemini API quota or try again later."
                        : "I'm temporarily experiencing technical difficulties. Please try again later.";

                    return new ChatResponse
                    {
                        Answer = answer,
                        TextTime = DateTime.Now
                    };
                }

                // Parse Gemini response structure
                if (!root.TryGetProperty("candidates", out var candidatesElement) || candidatesElement.GetArrayLength() == 0)
                {
                    _logger.LogError("No candidates found in Gemini API response: {Response}", responseJson);
                    return new ChatResponse
                    {
                        Answer = "I couldn't generate a response. Please try rephrasing your question.",

                        TextTime = DateTime.Now
                    };
                }

                var firstCandidate = candidatesElement[0];

                // Check if content exists
                if (!firstCandidate.TryGetProperty("content", out var contentElement))
                {
                    _logger.LogError("No content found in first candidate: {Response}", responseJson);
                    return new ChatResponse
                    {
                        Answer = "I couldn't process the response properly. Please try again.",

                        TextTime = DateTime.Now
                    };
                }

                // Check if parts array exists
                if (!contentElement.TryGetProperty("parts", out var partsElement) || partsElement.GetArrayLength() == 0)
                {
                    _logger.LogError("No parts found in content: {Response}", responseJson);
                    return new ChatResponse
                    {
                        Answer = "I couldn't generate content for your question. Please try again.",

                        TextTime = DateTime.Now
                    };
                }

                var firstPart = partsElement[0];

                // Check if text exists
                if (!firstPart.TryGetProperty("text", out var textElement))
                {
                    _logger.LogError("No text found in first part: {Response}", responseJson);
                    return new ChatResponse
                    {
                        Answer = "I couldn't generate text content. Please try again.",

                        TextTime = DateTime.Now
                    };
                }

                return new ChatResponse
                {
                    Answer = textElement.GetString() ?? "I couldn't generate a response. Please try again.",

                    TextTime = DateTime.Now
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON response from Gemini API");
                return new ChatResponse
                {
                    Answer = "I encountered a technical issue. Please try again.",

                    TextTime = DateTime.Now
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Gemini API failed");
                return new ChatResponse
                {
                    Answer = "I'm currently unable to connect to the service. Please try again later.",

                    TextTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ChatbotService.AskAsync");
                return new ChatResponse
                {
                    Answer = "I encountered an unexpected issue. Please try again.",

                    TextTime = DateTime.Now
                };
            }
        }
    }
}