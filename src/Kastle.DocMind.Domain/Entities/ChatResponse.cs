namespace Kastle.DocMind.Domain.Entities;
using System.Collections.Generic;
public class ChatResponse
{
    public string Answer{get;set;}=string.Empty;
    public List<Citation>Citations{get;set;}=new();
    public List<RetrievedChunk> RetrievedChunks { get; set; } = new();

    public TokenUsage TokensUsed { get; set; } = new();

    public long LatencyMs { get; set; }
}