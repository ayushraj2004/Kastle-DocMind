using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Kastle.DocMind.Api.Models;
namespace Kastle.DocMind.Api.Controllers;
[ApiController]
[Route("documents")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    public DocumentController(IDocumentService documentService)
    {
        _documentService=documentService;
    }
    [HttpPost]// uploading the file 
    public async Task<ActionResult<Document>>Upload([FromForm]UploadDocumentRequest request)//asynchronous Task
    {
        var file = request.File!;
        if(file==null || file.Length == 0)
        {
            return BadRequest("File is required");
        }
        var document =await _documentService.UploadAsync(file.OpenReadStream(),file.FileName,file.ContentType,file.Length);
        return CreatedAtAction(nameof(GetById),new {id=document.Id},document);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Document>>> GetAll()
    {
        var documents=await _documentService.GetAllAsync();
        return Ok(documents);
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Document>>GetById(Guid id)
    {
        var document=await _documentService.GetByIdAsync(id);
        if(document==null) return NotFound();
        return Ok(document);
    }
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult>Delete(Guid id)
    {
        var document =await _documentService.GetByIdAsync(id);
        if(document==null) return NotFound();
        await _documentService.DeleteAsync(id);
        return NoContent();

    }
}