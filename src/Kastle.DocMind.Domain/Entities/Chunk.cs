namespace Kastle.DocMind.Domain.Entities;
public class Chunk
{
    public Guid Id{get;set;}//unique id for chunk
    public Guid DocumentId{get;set;}//tell us which document this chunk belongs
    public int SequenceNumber{get;set;}//store order of chunk
    public string Text{get;set;}=string.Empty;//actual content of chunk
    public string? FileName{get;set;}
    public string? Section{get;set;}
    public int TokenCount{get;set;}

}