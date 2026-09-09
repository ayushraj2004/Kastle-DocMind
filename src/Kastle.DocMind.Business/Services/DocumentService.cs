using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Business.Interfaces;
using Microsoft.Extensions.Logging;
namespace Kastle.DocMind.Business.Services;
public class DocumentService : IDocumentService//business layer service that handle doc-related opr.
{
    private readonly IDocumentRepository _repository;//repository used to save and retrieve doc. metadata from db.
    private readonly IFileStorage _fileStorage;//storage service save and delete actual doc.
    private readonly ITextExtractorResolver _extractorResolver;//resovler used to select correct text extractor
    private readonly ILogger<DocumentService> _logger;


    //Constructor dependencies are provide usuing DI
    public DocumentService(IDocumentRepository repository,IFileStorage fileStorage,ITextExtractorResolver extractorResolver,ILogger<DocumentService>logger)
    {
        _repository=repository;
        _fileStorage=fileStorage;
        _extractorResolver=extractorResolver;
        _logger=logger;
    }
    //upload file extract its txt save it metadata
    public async Task<Document>UploadAsync(Stream fileStream,string fileName,string contentType,long size)
    {
        _logger.LogInformation("Upload document{FileName}",fileName);

        var filePath=await _fileStorage.SaveAsync(fileStream,fileName);//save actual file to file storage
        _logger.LogInformation("Document{FileName} save successfully",fileName);
        fileStream.Position=0;//reset the position so read file again 
        var extractor=_extractorResolver.Resolve(fileName);
        var text =await extractor.ExtractTextAsync(fileStream,fileName);
        var document =new Document// creating metadata 
        {
            Id=Guid.NewGuid(),
            FileName=fileName,
            Size=size,
            ContentType=contentType,
            UploadedAt=DateTime.UtcNow,
            Status="Uploaded",
            FilePath=filePath
        };
        var savedDocument= await _repository.AddAsync(document);
        _logger.LogInformation("Document{DocumentId} created successfully",document.Id);
        return savedDocument;

    }
    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
    public async Task<Document?>GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
    public async Task DeleteAsync(Guid id)
    {
        var document =await _repository.GetByIdAsync(id);
        if(document==null) return;
        await _fileStorage.DeleteAsync(document.FilePath);
        await _repository.DeleteAsync(document);    
    }

}   