using AgentCopilot.API.AgentHub;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AgentCopilot.API.Controllers;

[Route("api/chat")]
[ApiController]
public class ChatSummaryController : ControllerBase
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration _config;

    public ChatSummaryController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        httpClient = httpClientFactory.CreateClient();
        _config = configuration;
    }

    [HttpGet("/summary/{chatId}")]
    public async Task<IActionResult> GetSummary(string chatId)
    {
        if (!ChatMemoryStore.ChatMessages.TryGetValue(chatId, out var messages))
        {
            return NotFound("Chat NotFound ");
        }

        var prompt = "Summarize this support conversation between Customer and Agent:\n\n";

        foreach (var msg in messages.TakeLast(10))
        {
            prompt += $"{msg.Sender}: {msg.Message}\n";
        }

        var requestBody = new
        {
            messages = new[]
          {
                new { role = "system", content = "You are a helpful AI that summarizes support conversations." },
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(
          HttpMethod.Post,
          $"{_config["AzureOpenAI:Endpoint"]}/openai/deployments/{_config["AzureOpenAI:Deployment"]}/chat/completions?api-version={_config["AzureOpenAI:ApiVersion"]}"
      );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["AzureOpenAI:ApiKey"]);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);
        var resultContent = await response.Content.ReadAsStringAsync();

        dynamic result = JsonConvert.DeserializeObject(resultContent);
        string summary = result?.choices[0]?.message?.content ?? "Unable to generate summary.";

        return Ok(new { summary });


    }
}