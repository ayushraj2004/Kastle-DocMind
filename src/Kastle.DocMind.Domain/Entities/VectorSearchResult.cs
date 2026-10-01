namespace Kastle.DocMind.Domain.Entities;   
public class VectorSearchResult
{
    
    public Guid ChunkId{get;set;}
    public Guid DocumentId{get;set;}
    public string Text{get;set;}
    public string? FileName{get;set;}
    public string? Section{get;set;}
    public int SequenceNumber{get;set;}
    public float Score{get;set;}
}