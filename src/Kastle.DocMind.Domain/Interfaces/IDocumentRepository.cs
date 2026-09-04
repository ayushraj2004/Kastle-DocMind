using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;


public interface IDocumentRepository
{
    Task<Document>AddAsync(Document document);
    Task<IEnumerable<Document>>GetAllAsync();
    Task<Document?>GetByIdAsync(Guid id);
    Task DeleteAsync(Document document);
}