namespace Kastle.DocMind.Infrastructure.Data;

public class MongoDbSettings
{
    public string ConnectionString{get;set;}=string.Empty;
    public string DatabaseName{get;set;}=string.Empty;
    public string DocumentsCollectionName{get;set;}=string.Empty;
}