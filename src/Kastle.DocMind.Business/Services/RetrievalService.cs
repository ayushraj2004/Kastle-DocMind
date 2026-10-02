using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
namespace Kastle.DocMind.Business.Services;
public class RetrievalService : IRetrievalService
{
    private readonly IEmbeddingGenerator<string,Embedding<float>> _embeddingGenerator;
    private readonly IVectorStore _vectorStore;
    private const int TopK=5;
    public RetrievalService(IEmbeddingGenerator<string,Embedding<float>>embeddingGenerator,IVectorStore vectorStore)
    {
        _embeddingGenerator=embeddingGenerator;
        _vectorStore=vectorStore;
    }
    public async Task<IReadOnlyList<VectorSearchResult>>RetrieveAsync(string question,CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException("question cannot be empty",nameof(question));
        }
        var embedding=await _embeddingGenerator.GenerateAsync(question,cancellationToken:cancellationToken);
        var queryVector=embedding.Vector.ToArray();
        var result=await _vectorStore.SearchAsync(queryVector,TopK,cancellationToken:cancellationToken);
        return result;
    }
}