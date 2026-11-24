using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var apiBase = args.Length > 0 ? args[0] : "http://localhost:5144";
var questionsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "eval", "questions.json"));
var runsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "eval", "runs"));
Directory.CreateDirectory(runsDir);

Console.WriteLine($"DocMind Evaluation Harness");
Console.WriteLine($"API: {apiBase}");
Console.WriteLine($"Questions: {questionsPath}");
Console.WriteLine();

var questions = JsonSerializer.Deserialize<List<EvalQuestion>>(
    await File.ReadAllTextAsync(questionsPath),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    ?? throw new Exception("Failed to load questions.json");

using var http = new HttpClient { BaseAddress = new Uri(apiBase), Timeout = TimeSpan.FromSeconds(120) };

var results = new List<EvalResult>();
var latencies = new List<long>();

foreach (var q in questions)
{
    Console.Write($"[{q.Id}] {q.Question[..Math.Min(60, q.Question.Length)]}... ");

    var sw = Stopwatch.StartNew();
    ChatResponse? response = null;
    string? error = null;

    try
    {
        var payload = JsonSerializer.Serialize(new { question = q.Question });
        var httpResponse = await http.PostAsync("/chat",
            new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await httpResponse.Content.ReadAsStringAsync();
        response = JsonSerializer.Deserialize<ChatResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch (Exception ex)
    {
        error = ex.Message;
    }

    sw.Stop();
    var latencyMs = sw.ElapsedMilliseconds;
    latencies.Add(latencyMs);

    var answer = response?.Answer ?? string.Empty;
    var isRefusal = answer.Contains("doesn't cover", StringComparison.OrdinalIgnoreCase)
                 || answer.Contains("does not cover", StringComparison.OrdinalIgnoreCase)
                 || answer.Contains("not covered", StringComparison.OrdinalIgnoreCase);

    // Retrieval hit: expected doc present in retrieved chunks
    var retrievedDocIds = response?.RetrievedChunks?
        .Select(c => c.DocumentId.ToString())
        .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
    var retrievalHit = !string.IsNullOrEmpty(q.ExpectedSourceDocumentId)
        && retrievedDocIds.Contains(q.ExpectedSourceDocumentId);

    // Citation hit: expected doc present in citations
    var citationDocIds = response?.Citations?
        .Select(c => c.DocumentId.ToString())
        .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
    var citationHit = !string.IsNullOrEmpty(q.ExpectedSourceDocumentId)
        && citationDocIds.Contains(q.ExpectedSourceDocumentId);

    // Refusal correctness
    var refusalCorrect = q.Category == "out_of_scope" ? isRefusal : !isRefusal;

    // Keyword matching against expectedAnswerPoints
    var matchedPoints = q.ExpectedAnswerPoints
        .Where(p => answer.Contains(p, StringComparison.OrdinalIgnoreCase))
        .ToList();
    var keywordScore = q.ExpectedAnswerPoints.Count > 0
        ? (float)matchedPoints.Count / q.ExpectedAnswerPoints.Count
        : 1f;

    // Top score from retrieved chunks
    var topScore = response?.RetrievedChunks?.Select(c => c.Score).DefaultIfEmpty(0).Max() ?? 0;
    var tokenCount = (response?.TokensUsed?.Prompt ?? 0) + (response?.TokensUsed?.Completion ?? 0);

    var result = new EvalResult
    {
        Id = q.Id,
        Category = q.Category,
        Question = q.Question,
        Answer = answer,
        RetrievalHit = retrievalHit,
        CitationHit = citationHit,
        IsRefusal = isRefusal,
        RefusalCorrect = refusalCorrect,
        MatchedPoints = matchedPoints.Count,
        TotalPoints = q.ExpectedAnswerPoints.Count,
        KeywordScore = keywordScore,
        LatencyMs = latencyMs,
        TopScore = topScore,
        TokensUsed = tokenCount,
        ManualGrade = -1, // -1 = not graded yet
        Error = error
    };

    results.Add(result);

    var status = error != null ? "ERROR" : isRefusal ? "REFUSED" : "ANSWERED";
    Console.WriteLine($"{status} | hit={retrievalHit} | kw={matchedPoints.Count}/{q.ExpectedAnswerPoints.Count} | {latencyMs}ms");
}

// Summary metrics
var answerable = results.Where(r => r.Category != "out_of_scope").ToList();
var outOfScope = results.Where(r => r.Category == "out_of_scope").ToList();

var hitRate = answerable.Count > 0 ? (float)answerable.Count(r => r.RetrievalHit) / answerable.Count : 0;
var falseRefusals = answerable.Count(r => r.IsRefusal);
var falseAnswers = outOfScope.Count(r => !r.IsRefusal);
var avgKeyword = answerable.Count > 0 ? answerable.Average(r => r.KeywordScore) : 0;

var sorted = latencies.Order().ToList();
var p50 = sorted.Count > 0 ? sorted[sorted.Count / 2] : 0;
var p95 = sorted.Count > 0 ? sorted[(int)(sorted.Count * 0.95)] : 0;

// Write report
var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
var reportPath = Path.Combine(runsDir, $"eval-{timestamp}.md");

var sb = new StringBuilder();
sb.AppendLine($"# DocMind Evaluation Run — {timestamp} UTC");
sb.AppendLine();
sb.AppendLine("## Summary");
sb.AppendLine();
sb.AppendLine($"| Metric | Value |");
sb.AppendLine($"|--------|-------|");
sb.AppendLine($"| Total questions | {results.Count} |");
sb.AppendLine($"| Answerable | {answerable.Count} |");
sb.AppendLine($"| Out-of-scope | {outOfScope.Count} |");
sb.AppendLine($"| Retrieval hit rate @k | {hitRate:P0} ({answerable.Count(r => r.RetrievalHit)}/{answerable.Count}) |");
sb.AppendLine($"| False refusals (answerable refused) | {falseRefusals} |");
sb.AppendLine($"| False answers (out-of-scope answered) | {falseAnswers} |");
sb.AppendLine($"| Avg keyword score | {avgKeyword:P0} |");
sb.AppendLine($"| Latency p50 | {p50}ms |");
sb.AppendLine($"| Latency p95 | {p95}ms |");
sb.AppendLine();
sb.AppendLine("## Per-Question Results");
sb.AppendLine();
sb.AppendLine("| ID | Category | Retrieval Hit | Refusal OK | KW Score | Latency | Manual Grade | Answer (truncated) |");
sb.AppendLine("|----|----------|--------------|------------|----------|---------|-------------|-------------------|");

foreach (var r in results)
{
    var answerSnippet = r.Error ?? r.Answer[..Math.Min(80, r.Answer.Length)];
    answerSnippet = answerSnippet.Replace("|", "\\|").Replace("\n", " ");
    var grade = r.ManualGrade == -1 ? "—" : r.ManualGrade.ToString();
    sb.AppendLine($"| {r.Id} | {r.Category} | {(r.RetrievalHit ? "✅" : "❌")} | {(r.RefusalCorrect ? "✅" : "❌")} | {r.MatchedPoints}/{r.TotalPoints} | {r.LatencyMs}ms | {grade} | {answerSnippet} |");
}

sb.AppendLine();
sb.AppendLine("## Full Answers");
sb.AppendLine();
foreach (var r in results)
{
    sb.AppendLine($"### {r.Id} — {r.Category}");
    sb.AppendLine($"**Q:** {r.Question}");
    sb.AppendLine();
    sb.AppendLine($"**A:** {r.Answer}");
    sb.AppendLine();
    sb.AppendLine($"- Retrieval hit: {r.RetrievalHit} | Citation hit: {r.CitationHit} | Top score: {r.TopScore:F3} | Tokens: {r.TokensUsed} | Latency: {r.LatencyMs}ms");
    sb.AppendLine($"- Keyword matches: {r.MatchedPoints}/{r.TotalPoints} | Manual grade: {(r.ManualGrade == -1 ? "not graded" : r.ManualGrade.ToString())}");
    sb.AppendLine();
}

await File.WriteAllTextAsync(reportPath, sb.ToString());

Console.WriteLine();
Console.WriteLine("=== SUMMARY ===");
Console.WriteLine($"Retrieval hit rate: {hitRate:P0}");
Console.WriteLine($"False refusals:     {falseRefusals}");
Console.WriteLine($"False answers:      {falseAnswers}");
Console.WriteLine($"Avg keyword score:  {avgKeyword:P0}");
Console.WriteLine($"Latency p50/p95:    {p50}ms / {p95}ms");
Console.WriteLine();
Console.WriteLine($"Report saved: {reportPath}");

// --- Models ---

record EvalQuestion
{
    public string Id { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public List<string> ExpectedAnswerPoints { get; init; } = new();
    public string? ExpectedSourceDocumentId { get; init; }
    public string Category { get; init; } = "answerable";
}

record EvalResult
{
    public string Id { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool RetrievalHit { get; init; }
    public bool CitationHit { get; init; }
    public bool IsRefusal { get; init; }
    public bool RefusalCorrect { get; init; }
    public int MatchedPoints { get; init; }
    public int TotalPoints { get; init; }
    public float KeywordScore { get; init; }
    public long LatencyMs { get; init; }
    public float TopScore { get; init; }
    public int TokensUsed { get; init; }
    public int ManualGrade { get; set; }
    public string? Error { get; init; }
}

class ChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<CitationDto>? Citations { get; set; }
    public List<RetrievedChunkDto>? RetrievedChunks { get; set; }
    public TokenUsageDto? TokensUsed { get; set; }
    public long LatencyMs { get; set; }
}

class CitationDto
{
    public Guid DocumentId { get; set; }
    public Guid ChunkId { get; set; }
    public string? FileName { get; set; }
}

class RetrievedChunkDto
{
    public Guid DocumentId { get; set; }
    public float Score { get; set; }
}

class TokenUsageDto
{
    public int Prompt { get; set; }
    public int Completion { get; set; }
}
