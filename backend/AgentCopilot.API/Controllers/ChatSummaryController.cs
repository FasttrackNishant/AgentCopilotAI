using AgentCopilot.API.AgentHub;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using StackExchange.Redis;
using AgentCopilot.API.Services;

namespace AgentCopilot.API.Controllers;

[Route("api/chat")]
[ApiController]
public class ChatSummaryController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly RedisService _redis;

    public ChatSummaryController(IHttpClientFactory httpClientFactory, IConfiguration config, RedisService redis)
    {
        _httpClient = httpClientFactory.CreateClient();
        _config = config;
        _redis = redis;
    }

    [HttpGet("summary/{chatId}")]
    public async Task<IActionResult> GetSummary(string chatId)
    {
        var messages = await _redis.GetChatMessagesAsync(chatId);

        if (messages == null || !messages.Any())
            return NotFound("Chat not found or empty.");

        var recentMessages = messages.TakeLast(10);

        foreach (var item in recentMessages)
        {
            Console.WriteLine(item.Message);
        }

        var chatText = string.Join("\n", recentMessages.Select(m => $"{m.Sender}: {m.Message}"));

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are a helpful AI that summarizes support conversations." },
                new { role = "user", content = $"Summarize this support conversation:\n\n{chatText}" }
            }
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_config["AzureOpenAI:Endpoint"]}/openai/deployments/{_config["AzureOpenAI:Deployment"]}/chat/completions?api-version={_config["AzureOpenAI:ApiVersion"]}"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["AzureOpenAI:ApiKey"]);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"AI error: {error}");
        }

        var resultContent = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(resultContent);
        string summary = result?.choices[0]?.message?.content ?? "Unable to generate summary.";

        return Ok(new { summary });
    }
}