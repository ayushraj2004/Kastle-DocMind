using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Business.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
namespace Kastle.DocMind.Business.Services;
public class ChatService : IChatService
{
    private readonly PromptBuilder _promptBuilder;
    private readonly ContextBudgeter _contextBudgeter;
    private readonly IChatClient _chatClient;
    private readonly ILogger<ChatService> _logger;
    private readonly ConversationOptions _options;
    public ChatService(PromptBuilder promptBuilder, ContextBudgeter contextBudgeter, IChatClient chatClient, ILogger<ChatService> logger, IOptions<ConversationOptions> options)
    {
        _promptBuilder = promptBuilder;
        _contextBudgeter = contextBudgeter;
        _chatClient = chatClient;
        _logger = logger;
        _options = options.Value;
    }
    public async Task<ChatResult> GetResponseAsync(string question, IReadOnlyList<VectorSearchResult> chunks, IReadOnlyList<ConversationTurn> history, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Chat question: {Question}; Received chunks: {Count}", question, chunks.Count);
        if (chunks.Count == 0)
            return new ChatResult { Answer = "The documentation does not cover this question." };

        var selectedChunks = _contextBudgeter.LimitChunks(chunks);
        if (selectedChunks.Count == 0)
            return new ChatResult { Answer = "The documentation does not cover this question." };

        var trimmedHistory = history
            .TakeLast(_options.MaxHistoryTurns)
            .ToList();

        var finalPrompt = _promptBuilder.Build(question, selectedChunks, trimmedHistory);

        var sw = Stopwatch.StartNew();
        var response = await _chatClient.GetResponseAsync(finalPrompt, cancellationToken: cancellationToken);
        _logger.LogInformation("[TIMING] Ollama chat generation: {Ms}ms", sw.ElapsedMilliseconds);

        return new ChatResult
        {
            Answer = response.Text ?? string.Empty,
            Chunks = selectedChunks,
            TokensUsed = new TokenUsage
            {
                Prompt = (int)(response.Usage?.InputTokenCount ?? 0),
                Completion = (int)(response.Usage?.OutputTokenCount ?? 0)
            }
        };
    }
}