using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Kastle.DocMind.Api.Models;
namespace Kastle.DocMind.Api.Controllers;
using FluentValidation;
[ApiController]
[Route("documents")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IValidator<UploadDocumentRequest> _validator;
    private readonly ILogger<DocumentController> _logger;
    public DocumentController(IDocumentService documentService,IValidator<UploadDocumentRequest>validator,ILogger<DocumentController>logger)
    {
        _documentService=documentService;
        _validator=validator;
        _logger=logger;
    }
    [HttpPost]// uploading the file 
    public async Task<ActionResult<Document>>Upload([FromForm]UploadDocumentRequest request)//asynchronous Task
    {

        _logger.LogInformation("received document uploade request for {FileName}",request.File?.FileName);
        var validationResult=await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors=validationResult.Errors.Select(error=>error.ErrorMessage).ToList();
            return BadRequest(new{message="Validation Failed.",errors});
            
        }
        var file = request.File!;
        var document =await _documentService.UploadAsync(file.OpenReadStream(),file.FileName,file.ContentType,file.Length);
        return CreatedAtAction(nameof(GetById),new {id=document.Id},document);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Document>>> GetAll([FromQuery]int page=1,[FromQuery]int pageSize=10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("page must be greatere than 0 & must b/w 1 and 100");
        }
        var documents=await _documentService.GetAllAsync(page,pageSize);
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