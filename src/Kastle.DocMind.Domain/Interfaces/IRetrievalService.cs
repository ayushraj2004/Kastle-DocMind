using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;
public interface IRetrievalService
{
    Task<IReadOnlyList<VectorSearchResult>>RetrieveAsync(string question,CancellationToken cancellationToken=default);
}