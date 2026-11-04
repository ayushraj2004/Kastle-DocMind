
namespace Kastle.DocMind.Domain.Entities;

public class EvalRunReport
{
    public Guid RunId { get; set; } = Guid.NewGuid();
    public DateTimeOffset RunDate { get; set; } = DateTimeOffset.UtcNow;
    public string ModelName { get; set; } = string.Empty;
    public Dictionary<string, string> Configuration { get; set; } = new();
    public List<EvalResult> Results { get; set; } = new();
    public int TotalQuestions { get; set; }
    public int RetrievalHits { get; set; }
    public double RetrievalHitRate { get; set; }
    public int CorrectRefusals { get; set; }
    public int FalseRefusals { get; set; }
    public int FalseAnswers { get; set; }
    public double AverageLatencyMs { get; set; }
    public double P50LatencyMs { get; set; }
    public double P95LatencyMs { get; set; }
    public double AverageTokensUsed { get; set; }
}
