namespace AgentCopilot.API.AgentHub;

public class ChatMessage
{
    public string Sender { get; set; }
    public string Message { get; set; }
    public string TimeStamp { get; set; }

}

public static class ChatMemoryStore
{
    public static Dictionary<string, List<ChatMessage>> ChatMessages = new();
}
