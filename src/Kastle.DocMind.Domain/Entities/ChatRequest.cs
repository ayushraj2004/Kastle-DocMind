using Kastle.DocMind.Domain.Entities;
using System.Text.Json.Serialization;
namespace Kastle.DocMind.Domain.Entities;
public class ChatRequest
{
    public string Question { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ConversationTurn>? ConversationHistory { get; set; }
}