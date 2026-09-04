namespace Kastle.DocMind.Domain.Entities;

public class Document
{
    public Guid Id {get; set;}
    public string FileName {get;set;}=string.Empty;
    public long Size{get;set;}
    public string ContentType{get;set;}=string.Empty;
    public DateTime UploadedAt{get;set;}
    public string Status {get;set;}=string.Empty;
    public string FilePath{get;set;}=string.Empty;
}