using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;

/// <summary>
/// Defines the contract for a document repository.
/// </summary>
public interface IDocumentRepository
{
    Task<Document>AddAsync(Document document);
    Task<IEnumerable<Document>>GetAllAsync(int page,int pageSize);
    Task<Document?>GetByIdAsync(Guid id);
    Task DeleteAsync(Document document);
    Task UpdateAsync(Document document);
}