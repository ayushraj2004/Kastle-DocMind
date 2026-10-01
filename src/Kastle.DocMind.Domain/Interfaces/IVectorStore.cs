using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;
public interface IVectorStore
{
    Task EnsureCollectionAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Chunk>>GetChunksByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task UpsertAsync(IReadOnlyList<Chunk> chunks,IReadOnlyList<float[]>embeddings,CancellationToken cancellationToken=default);
    Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorSearchResult>> SearchAsync(float[]queryVector,int topK,float? minScore=null,CancellationToken cancellationToken=default);
    
    }