using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;
namespace Kastle.DocMind.Infrastructure.VectorStore;
public class QdrantVectorStore : IVectorStore
{
    private readonly QdrantClient _client;
    private const string CollectionName="docmind_chunks";
    private const ulong VectorSize = 768;
    public QdrantVectorStore(QdrantClient client)
    {
        _client=client;
    }
    public async Task EnsureCollectionAsync(CancellationToken cancellationToken= default)
    {
        var collections=await _client.ListCollectionsAsync(cancellationToken: cancellationToken);
        if (collections.Contains(CollectionName))
        {
            return;
        }
        await _client.CreateCollectionAsync(CollectionName,new VectorParams{
            Size=VectorSize,
            Distance=Distance.Cosine
        },
        cancellationToken: cancellationToken);
    }
    public async Task UpsertAsync(IReadOnlyList<Chunk>chunks,IReadOnlyList<float[]>embeddings,CancellationToken cancellationToken = default)
    {
        if(chunks.Count!=embeddings.Count)
        {
            throw new ArgumentException("chunks and embeddings must have the same length");
        }
        var points =new List<PointStruct>();
        for(int i = 0; i < chunks.Count; i++)
        {
            var chunk=chunks[i];
            points.Add(new PointStruct{
                Id=new PointId { Uuid = chunk.Id.ToString() },Vectors=new Vectors{Vector=new Vector{Data={embeddings[i]}}},Payload ={
                    ["documentId"]=chunk.DocumentId.ToString(),
                    ["chunkId"]=chunk.Id.ToString(),
                    ["sequenceNumber"]=chunk.SequenceNumber,
                    ["text"]=chunk.Text,
                    ["fileName"]=chunk.FileName ?? string.Empty,
                    ["section"]=chunk.Section ?? string.Empty
                }
            });
        }
        await _client.UpsertAsync(CollectionName,points,cancellationToken: cancellationToken);
    }
    public async Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await _client.DeleteAsync(CollectionName,new Filter
        {
            Must =
            {
                new Condition
                {
                    Field=new FieldCondition
                    {
                        Key="documentId",
                        Match=new Match
                        {
                            Keyword=documentId.ToString()
                        }
                    }
                }
            }
        },cancellationToken:cancellationToken);
    }
    public async Task<IReadOnlyList<Chunk>>GetChunksByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var result=await _client.QueryAsync(CollectionName,query:null,filter:new Filter
        {
            Must =
            {
                new Condition
                {
                    Field=new FieldCondition
                    {
                        Key="documentId",
                        Match=new Match
                        {
                            Keyword=documentId.ToString()
                        }
                    }
                }
            }
        },
        limit:1000,cancellationToken:cancellationToken);
        return result.Select(point=>new Chunk
        {
            Id=Guid.Parse(point.Payload["chunkId"].StringValue),
            DocumentId=Guid.Parse(point.Payload["documentId"].StringValue),
            SequenceNumber=(int)point.Payload["sequenceNumber"].IntegerValue,
            Text=point.Payload["text"].StringValue,
            FileName=point.Payload["fileName"].StringValue,
            Section=point.Payload["section"].StringValue

        })
        .OrderBy(chunk=>chunk.SequenceNumber)
        .ToList();
    }
}