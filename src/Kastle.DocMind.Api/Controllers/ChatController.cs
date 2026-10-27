using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
namespace Kastle.DocMind.Api.Controllers;
[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IRetrievalService _retrievalService;
    private readonly IChatService _chatService;
    public ChatController(IChatService chatService,IRetrievalService retrievalService)
    {
        _chatService=chatService;
        _retrievalService=retrievalService;
    }
    [HttpPost]
    public async Task<ActionResult<ChatResponse>>Chat([FromBody]ChatRequest request,CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Que is required");
        }
        var chunks=await _retrievalService.RetrieveAsync(request.Question, request.ConversationHistory, cancellationToken);
        var result=await _chatService.GetResponseAsync(request.Question,chunks,request.ConversationHistory ?? [],cancellationToken);
        var isRefusal = result.Answer.Contains("doesn't cover", StringComparison.OrdinalIgnoreCase)
            || result.Answer.Contains("does not cover", StringComparison.OrdinalIgnoreCase);
        var citations = isRefusal
            ? new List<Citation>()
            : result.Chunks
                .Select((chunk, index) => new { chunk, number = index + 1 })
                .Where(x => result.Answer.Contains($"[{x.number}]"))
                .Select(x => new Citation { Marker = x.number, DocumentId = x.chunk.DocumentId, ChunkId = x.chunk.ChunkId, FileName = x.chunk.FileName, Section = x.chunk.Section })
                .ToList();
                stopwatch.Stop();
        return Ok(new ChatResponse{Answer=result.Answer,Citations=citations,RetrievedChunks=result.Chunks.Select(chunk=> 
        new RetrievedChunk
        {
            ChunkId=chunk.ChunkId,
            DocumentId=chunk.DocumentId,
            FileName=chunk.FileName,
            Section=chunk.Section,
            Text=chunk.Text,
            Score=chunk.Score
        }).ToList(),TokensUsed=result.TokensUsed,LatencyMs=stopwatch.ElapsedMilliseconds});
    }

}