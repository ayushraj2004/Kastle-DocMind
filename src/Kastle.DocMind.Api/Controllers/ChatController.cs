using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Kastle.DocMind.Api.Controllers;
[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    public ChatController(IChatService chatService)
    {
        _chatService=chatService;
    }
    [HttpPost]
    public async Task<ActionResult<ChatResponse>>Chat([FromBody]ChatRequest request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Que is required");
        }
        var answer=await _chatService.GetResponseAsync(request.Question,cancellationToken);
        return Ok(new ChatResponse{Answer=answer});
    }

}