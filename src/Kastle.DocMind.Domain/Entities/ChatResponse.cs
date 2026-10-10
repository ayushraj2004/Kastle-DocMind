namespace Kastle.DocMind.Domain.Entities;
public class ChatResponse
{
    public string Answer{get;set;}=string.Empty;
    public List<Citation>Citations{get;set;}=new();
}