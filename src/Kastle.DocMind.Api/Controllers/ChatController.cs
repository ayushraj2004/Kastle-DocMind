using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
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
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Que is required");
        }
        var chunks=await _retrievalService.RetrieveAsync(request.Question,cancellationToken);
        var answer=await _chatService.GetResponseAsync(request.Question,chunks,cancellationToken);
        var isFallback=string.Equals(answer.Trim().Trim('"'),"The document doesn't cover this question",StringComparison.OrdinalIgnoreCase);
        var citations=chunks.Select((chunk,index)=>new Citation{Number=index+1,DocumentId=chunk.DocumentId,ChunkId=chunk.ChunkId,FileName=chunk.FileName,Section=chunk.Section}).ToList();
        return Ok(new ChatResponse{Answer=answer,Citations=citations});
    }

}