namespace Kastle.DocMind.Domain.Entities;
public class Chunk
{
    public Guid Id{get;set;}
    public Guid DocumentId{get;set;}
    public int SequenceNumber{get;set;}
    public string Text{get;set;}
    public string? FileName{get;set;}
    public string? Section{get;set;}
    public int TokenCount{get;set;}

}