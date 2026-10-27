namespace Kastle.DocMind.Domain.Entities;
public class ChatRequest
{
    public string Question{get;set;}=string.Empty;
    public List<ConversationTurn>ConversationHistory{get;set;}=new();
}