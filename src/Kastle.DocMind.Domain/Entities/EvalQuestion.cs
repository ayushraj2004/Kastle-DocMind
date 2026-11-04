
using System.Text.Json.Serialization;
namespace Kastle.DocMind.Domain.Entities;
public class EvalQuestion
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;
    [JsonPropertyName("expectedAnswerPoints")]
    public List<string> ExpectedAnswerPoints { get; set; } = new();
    [JsonPropertyName("expectedSourceDocumentId")]
    public string? ExpectedSourceDocumentId { get; set; }
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}
