
namespace Kastle.DocMind.Domain.Entities;
public class EvalResult
{
    public string QuestionId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string? ExpectedSourceDocumentId { get; set; }
    public List<string> RetrievedDocumentIds { get; set; } = new();
    public bool RetrievalHit { get; set; }
    public string ActualAnswer { get; set; } = string.Empty;
    public bool WasRefusal { get; set; }
    public bool RefusalCorrect { get; set; }
    public bool CitationCorrect { get; set; }
    public List<string> MatchedAnswerPoints { get; set; } = new();
    public int? ManualScore { get; set; }
    public long LatencyMs { get; set; }
    public int? TokensUsed { get; set; }
}
