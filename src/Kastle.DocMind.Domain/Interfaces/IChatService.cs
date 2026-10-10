using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;
public interface IChatService
{
    Task<string>GetResponseAsync(string question,IReadOnlyList<VectorSearchResult>chunks,CancellationToken cancellationToken=default);
}