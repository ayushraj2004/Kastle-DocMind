using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
namespace Kastle.DocMind.Business.Services;
public class ChatService : IChatService
{
    private readonly IRetrievalService _retrievalService;
    private readonly PromptBuilder _promptBuilder;
    private readonly ContextBudgeter _contextBudgeter;
    private readonly IChatClient _chatClient;
    public ChatService(IRetrievalService retrievalService,PromptBuilder promptBuilder,ContextBudgeter contextBudgeter,IChatClient chatClient)
    {
        _retrievalService=retrievalService;
        _promptBuilder=promptBuilder;
        _contextBudgeter=contextBudgeter;
        _chatClient=chatClient;
    }
    public async Task<string>GetResponseAsync(string prompt,CancellationToken cancellationToken = default)
    {
        var chunks=await _retrievalService.RetrieveAsync(prompt,cancellationToken);
        var selectedChunks=_contextBudgeter.LimitChunks(chunks);
        var finalPrompt=_promptBuilder.Build(prompt,selectedChunks);
        var response=await _chatClient.GetResponseAsync(finalPrompt,cancellationToken:cancellationToken);
        return response.Text??(string.Empty);
    }
}