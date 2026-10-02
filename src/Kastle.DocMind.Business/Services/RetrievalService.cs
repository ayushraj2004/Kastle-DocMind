using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Business.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
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
        {
            throw new ArgumentException("question cannot be empty",nameof(question));
        }
        var embedding=await _embeddingGenerator.GenerateAsync(question,cancellationToken:cancellationToken);
        var queryVector=embedding.Vector.ToArray();
        var results=await _vectorStore.SearchAsync(queryVector,_options.TopK,_options.MinScore,cancellationToken);
        _logger.LogInformation("retrieved{count}chunk for question.TopK:{TopK}",results.Count,_options.TopK);
        foreach(var result in results)
        {
            _logger.LogInformation("retrieved chunk:{chunkId}from document{documentId}"+"with score:{score}",result.ChunkId,result.DocumentId,result.Score);
        }
        return results;
    }
}