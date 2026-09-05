using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// here adding the DocRepo class implement the IDocRepo interface

namespace Kastle.DocMind.Infrastructure.Repositories;
public class DocumentRepository : IDocumentRepository
{
    private readonly IMongoCollection<Document> _documents; // class is working with Mongo Collection

    //constructor to intialize mongo collection
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
        return await _documents.Find(_=> true).ToListAsync();// find all document in collection
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _documents.Find(document => document.Id == id).FirstOrDefaultAsync();// find by Id
    }
    public async Task DeleteAsync(Document document)
    {
        await _documents.DeleteOneAsync(item=>item.Id==document.Id);// delete by Id
    }
}