using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Business.Interfaces;

namespace Kastle.DocMind.Business.Services;
public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IFileStorage _fileStorage;
    private readonly ITextExtractorResolver _extractorResolver;
    public DocumentService(IDocumentRepository repository,IFileStorage fileStorage,ITextExtractorResolver extractorResolver)
    {
        _repository=repository;
        _fileStorage=fileStorage;
        _extractorResolver=extractorResolver;
    }
    public async Task<Document>UploadAsync(Stream fileStream,string fileName,string contentType,long size)
    {
        var filePath=await _fileStorage.SaveAsync(fileStream,fileName);
        fileStream.Position=0;
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
        return await _repository.AddAsync(document);//save metadata to mongodb

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