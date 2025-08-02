using AgentCopilot.API.Services;
using Microsoft.AspNetCore.SignalR;

namespace AgentCopilot.API.AgentHub;
public class ChatHub : Hub
{
    private readonly RedisService _redis;

    public ChatHub(RedisService redisService)
    {
        _redis = redisService;
    }
    public async Task JoinChat(string chatId)
    {
        Console.WriteLine($"Joining: {Context.ConnectionId} to {chatId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(string chatId, string sender, string message, string timestamp)
    {
        Console.WriteLine($"[{sender}] sending to group {chatId}: {message}");


        await Clients.Group(chatId).SendAsync("ReceiveMessage", sender, message, timestamp);

        await _redis.SaveMessageAsync(chatId, sender, message);
    }

    public async Task<List<string>> LoadHistory(string chatId)
    {
        return await _redis.GetMessagesAsync(chatId);
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
}
