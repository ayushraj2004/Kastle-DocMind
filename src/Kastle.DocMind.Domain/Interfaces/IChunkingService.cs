using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Domain.Interfaces;
public interface IChunkingService     //ichunkingservice is method which take text and return a collection of chunk
{
    IReadOnlyList<Chunk> ChunkText(Guid documentId,string text,string? FileName=null,string? section=null);   
}
