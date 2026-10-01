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
                    ["section"]=chunk.Section ?? string.Empty,
                    ["tokenCount"] = chunk.TokenCount
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
            Section=point.Payload["section"].StringValue,
            TokenCount = (int)point.Payload["tokenCount"].IntegerValue

        })
        .OrderBy(chunk=>chunk.SequenceNumber)
        .ToList();
        
    }
    public async Task<IReadOnlyList<VectorSearchResult>>SearchAsync(float[]queryVector,int topK,float?minScore=null,CancellationToken cancellationToken = default)
    {
        if (queryVector == null || queryVector.Length != (int)VectorSize)
        {
            throw new ArgumentException($"queryVector must contain{VectorSize}diamension",nameof(queryVector));
        }
        if (topK <= 0)
        {
            throw new ArgumentException(nameof(topK),"topK must be greater than zero");
        }
        var results=await _client.SearchAsync(collectionName:CollectionName,vector:queryVector,limit:(ulong)topK,scoreThreshold:minScore,cancellationToken:cancellationToken);
        return results.Select(point=>new VectorSearchResult{
            ChunkId = Guid.Parse(point.Payload["chunkId"].StringValue),
            DocumentId = Guid.Parse(point.Payload["documentId"].StringValue),
            Text = point.Payload["text"].StringValue,
            FileName = point.Payload["fileName"].StringValue,
            Section = point.Payload["section"].StringValue,
            SequenceNumber = (int)point.Payload["sequenceNumber"].IntegerValue,
            Score = point.Score
        }).ToList();
    }
    
}