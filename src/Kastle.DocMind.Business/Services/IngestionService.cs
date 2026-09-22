using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
namespace Kastle.DocMind.Business.Services;
public class IngestionService
{
    private readonly IVectorStore _vectorStore;
    private readonly IChunkingService _chunkingService;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    public IngestionService(IChunkingService chunkingService,IVectorStore vectorStore,IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator)
    {
        _chunkingService=chunkingService;
        _vectorStore=vectorStore;
        _embeddingGenerator=embeddingGenerator;
    }
    public async Task ProcessAsync(Guid documentId,string text,string? fileName=null,CancellationToken cancellationToken = default)
    {
        //create chunks
        var chunks=_chunkingService.ChunkText(documentId,text,fileName);
        if(chunks.Count==0) return;
        // qdrant collection exist
        await _vectorStore.EnsureCollectionAsync(cancellationToken);
        //generate embeddings
        var embeddings=new List<float[]>();
        foreach(var chunk in chunks)
        {
            var result=await _embeddingGenerator.GenerateAsync(chunk.Text,cancellationToken: cancellationToken);
            embeddings.Add(result.Vector.ToArray());
        }
        //store vector in qdrant
        await _vectorStore.UpsertAsync(chunks,embeddings,cancellationToken);    
    }
}