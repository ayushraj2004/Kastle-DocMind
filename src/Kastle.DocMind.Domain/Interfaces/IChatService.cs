using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;
public interface IChatService
{
    Task<ChatResult> GetResponseAsync(string question, IReadOnlyList<VectorSearchResult> chunks, IReadOnlyList<ConversationTurn> history, CancellationToken cancellationToken = default);
}