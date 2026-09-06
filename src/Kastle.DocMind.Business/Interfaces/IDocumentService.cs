using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Business.Interfaces;
public interface IDocumentService// define the opr. which doc. service provide
{
    Task<Document>UploadAsync(Stream fileStream,string fileName,string contentType,long size);//upload document and return save doc.
    Task<IEnumerable<Document>>GetAllAsync();// get all doc.
    Task<Document?>GetByIdAsync(Guid id);//get by id and if doc .not found return null
    Task DeleteAsync(Guid id);// delete doc.
}