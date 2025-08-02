using System.Text.Json;
using AgentCopilot.API.AgentHub;
using StackExchange.Redis;
namespace AgentCopilot.API.Services;

public class RedisService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    public RedisService(IConfiguration config)
    {
        var redisHost = config["Redis:Host"] ?? "localhost:6379";
        _redis = ConnectionMultiplexer.Connect(redisHost);
        _db = _redis.GetDatabase();
    }

    public async Task SaveMessageAsync(string chatId, string sender, string message)
    {
        var msg = new { sender, message, timestamp = DateTime.UtcNow.ToString("o") };
        var serialized = JsonSerializer.Serialize(msg);
        await _db.ListRightPushAsync(chatId, serialized);
    }

    public async Task<List<string>> GetMessagesAsync(string chatId)
    {
        var entries = await _db.ListRangeAsync(chatId, 0, -1);
        return entries.Select(e => e.ToString()).ToList();
    }

    public async Task ClearChatAsync(string chatId)
    {
        await _db.KeyDeleteAsync(chatId);
    }
    public async Task<List<ChatMessage>> GetChatMessagesAsync(string chatId)
{
    var rawMessages = await _db.ListRangeAsync(chatId);
      
    var messages = new List<ChatMessage>();

    foreach (var raw in rawMessages)
    {
        try
        {
            var msg = JsonSerializer.Deserialize<ChatMessage>(raw!);
            if (msg != null)
                messages.Add(msg);
        }
        catch { /* Handle bad JSON, log if needed */ }
    }

    return messages;
}
}