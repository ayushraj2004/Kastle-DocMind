using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Business.Interfaces;
public interface IDocumentService
{
    Task<Document>UploadAsync(Stream fileStream,string fileName,string contentType,long size);
    Task<IEnumerable<Document>>GetAllAsync();
    Task<Document?>GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}