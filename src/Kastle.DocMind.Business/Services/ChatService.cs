using Kastle.DocMind.Domain.Entities;
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
    public async Task<string>GetResponseAsync(string question,IReadOnlyList<VectorSearchResult>chunks,CancellationToken cancellationToken = default)
    {
        if (chunks.Count == 0)
        {
            return "The document doesn't cover question.";
        }
        var selectedChunks=_contextBudgeter.LimitChunks(chunks);
        if (selectedChunks.Count == 0)
        {
            return "The document doesn't cover question";
        }
        var finalPrompt=_promptBuilder.Build(question,selectedChunks);
        var response=await _chatClient.GetResponseAsync(finalPrompt,cancellationToken:cancellationToken);
        return response.Text??(string.Empty);
    }
}