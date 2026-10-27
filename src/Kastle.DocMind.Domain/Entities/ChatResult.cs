
using System;
using System.Collections.Generic;

namespace Kastle.DocMind.Domain.Entities;

public class ChatResult
{
    public string Answer { get; set; } = string.Empty;
    public IReadOnlyList<VectorSearchResult> Chunks { get; set; } = Array.Empty<VectorSearchResult>();
    public TokenUsage TokensUsed { get; set; } = new();
}
