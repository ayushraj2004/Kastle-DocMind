using Kastle.DocMind.Domain.Interfaces;
namespace Kastle.DocMind.Business.Services;
public class ChatService : IChatService
{
    public Task<string>GetResponseAsync(string prompt,CancellationToken cancellationToken = default)
    {
        return Task.FromResult(string.Empty);
    }
}