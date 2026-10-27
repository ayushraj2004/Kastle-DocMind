using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Business.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace Kastle.DocMind.Business.Services;

public class RetrievalService : IRetrievalService
{
    private readonly IEmbeddingGenerator<string,Embedding<float>> _embeddingGenerator;
    private readonly IVectorStore _vectorStore;
    private readonly RetrievalOptions _options;
    private readonly ILogger<RetrievalService> _logger;
    public RetrievalService(IEmbeddingGenerator<string,Embedding<float>>embeddingGenerator,IVectorStore vectorStore,IOptions<RetrievalOptions>options,ILogger<RetrievalService>logger)
    {
        _embeddingGenerator=embeddingGenerator;
        _vectorStore=vectorStore;
        _options=options.Value;
        _logger=logger;
    }
    public async Task<IReadOnlyList<VectorSearchResult>>RetrieveAsync(string question,CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("question cannot be empty",nameof(question));

        var sw = Stopwatch.StartNew();
        var embedding=await _embeddingGenerator.GenerateAsync(question,cancellationToken:cancellationToken);
        _logger.LogInformation("[TIMING] Embedding generation: {Ms}ms", sw.ElapsedMilliseconds);

        sw.Restart();
        var queryVector=embedding.Vector.ToArray();
        var results=await _vectorStore.SearchAsync(queryVector,_options.TopK,_options.MinScore,cancellationToken);
        _logger.LogInformation("[TIMING] Qdrant search: {Ms}ms — returned {Count} chunks", sw.ElapsedMilliseconds, results.Count);

        return results;
    }
}