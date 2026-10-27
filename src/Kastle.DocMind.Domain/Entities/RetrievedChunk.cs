namespace Kastle.DocMind.Domain.Entities;
public class RetrievedChunk
{
    public Guid ChunkId{get;set;}
    public Guid DocumentId { get; set; }
    public string? FileName{get;set;}
    public string? Section { get; set; }
    public string Text{get;set;}=string.Empty;
    public float Score{get;set;}

}