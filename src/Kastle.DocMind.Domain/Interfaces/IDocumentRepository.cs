using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;

/// <summary>
/// Defines the contract for a document repository.
/// </summary>
public interface IDocumentRepository
{
    Task<Document>AddAsync(Document document);
    Task<IEnumerable<Document>>GetAllAsync();
    Task<Document?>GetByIdAsync(Guid id);
    Task DeleteAsync(Document document);
}