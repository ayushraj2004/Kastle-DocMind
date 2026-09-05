using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Kastle.DocMind.Infrastructure.Repositories;
public class DocumentRepository : IDocumentRepository
{
    private readonly IMongoCollection<Document> _documents;
    public DocumentRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoClient =new MongoClient(settings.Value.ConnectionString);

        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _documents=database.GetCollection<Document>(settings.Value.DocumentsCollectionName);

    }
    public async Task<Document>AddAsync(Document document)
    {
        await _documents.InsertOneAsync(document);
        return document;
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _documents.Find(_=> true).ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _documents.Find(document => document.Id == id).FirstOrDefaultAsync();
    }
    public async Task DeleteAsync(Document document)
    {
        await _documents.DeleteOneAsync(item=>item.Id==document.Id);
    }
}