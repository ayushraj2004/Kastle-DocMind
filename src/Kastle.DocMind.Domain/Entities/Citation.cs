namespace Kastle.DocMind.Domain.Entities;
public class Citation
{
    public int Number { get; set; }
    public Guid DocumentId{get;set;}
    public Guid ChunkId{get;set;}
    public string? FileName{get;set;}
    public string? Section{get;set;}
}