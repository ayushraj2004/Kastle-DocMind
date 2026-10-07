using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
namespace Kastle.DocMind.Business.Services;
public class ChatService : IChatService
{
    private readonly IChatClient _chatClient;
    public ChatService(IChatClient chatClient)
    {
        _chatClient=chatClient;
    }
    public async Task<string>GetResponseAsync(string prompt,CancellationToken cancellationToken = default)
    {
        var response=await _chatClient.GetResponseAsync(prompt,cancellationToken:cancellationToken);
        return response.Text??(string.Empty);
    }
}