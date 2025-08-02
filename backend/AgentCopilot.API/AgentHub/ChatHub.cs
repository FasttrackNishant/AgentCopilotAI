using Microsoft.AspNetCore.SignalR;

namespace AgentCopilot.API.AgentHub;
public class ChatHub : Hub
{
    public async Task JoinChat(string chatId)
    {
        Console.WriteLine($"Joining: {Context.ConnectionId} to {chatId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(string chatId, string sender, string message,string timestamp)
    {
        Console.WriteLine($"[{sender}] sending to group {chatId}: {message}");

        var msg = new ChatMessage
        {
            Sender = sender,
            Message = message,
            TimeStamp = timestamp
        };

        if (!ChatMemoryStore.ChatMessages.ContainsKey(chatId))
        {
            ChatMemoryStore.ChatMessages[chatId] = new List<ChatMessage>();
        }

        ChatMemoryStore.ChatMessages[chatId].Add(msg);

        await Clients.Group(chatId).SendAsync("ReceiveMessage", sender, message,timestamp);
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
}
