namespace AgentCopilot.API.AgentHub;

using System.Text.Json.Serialization;

public class ChatMessage
{
    [JsonPropertyName("sender")]
    public string Sender { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;
}

public static class ChatMemoryStore
{
    public static Dictionary<string, List<ChatMessage>> ChatMessages = new();
}
