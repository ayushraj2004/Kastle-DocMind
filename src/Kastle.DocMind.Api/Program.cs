using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Infrastructure.Data;
using Kastle.DocMind.Infrastructure.Repositories;
using Kastle.DocMind.Infrastructure.TextExtraction;
using Kastle.DocMind.Business.Interfaces;
using Kastle.DocMind.Business.Services;
using Kastle.DocMind.Infrastructure.Storage;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using FluentValidation;
using Kastle.DocMind.Api.Validators;
using Kastle.DocMind.Api.Middleware;
using Serilog;
using Kastle.DocMind.Business.Options;
using Kastle.DocMind.Domain.Entities;
using Microsoft.Extensions.AI;
using OllamaSharp;
using Qdrant.Client;
using Kastle.DocMind.Infrastructure.VectorStore;
using System.Threading.Channels;
using Kastle.DocMind.Business.Workers;

Log.Logger=new LoggerConfiguration().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorageSettings"));


// here DI for MongoDbSettings and DocumentRepository
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<ITextExtractor, TxtTextExtractor>();
builder.Services.AddScoped<ITextExtractor, MarkdownTextExtractor>();

builder.Services.AddScoped<ITextExtractorResolver,TextExtractorResolver>();//registering the file extractor 

// register the DI 
builder.Services.AddScoped<IDocumentService,DocumentService>();

// File storage
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

// register the fluentvalidator
builder.Services.AddValidatorsFromAssemblyContaining<UploadDocumentRequestValidator>();



//registering the Bson serializer because mongodb read only the binary json file (BSON)
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));


//register the chunking option service 
builder.Services.Configure<ChunkingOptions>(builder.Configuration.GetSection("Chunking"));
builder.Services.AddScoped<IChunkingService, ChunkingService>();

// add ollama registration
var ollamaBaseUrl=builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
var embeddingModel=builder.Configuration["Ollama:EmbeddingModel"] ?? "nomic-embed-text";
builder.Services.AddSingleton<IEmbeddingGenerator<string,Embedding<float>>>(
    new OllamaApiClient(
        new Uri(ollamaBaseUrl),
        embeddingModel));

//register qdrant
builder.Services.AddSingleton<QdrantClient>(new QdrantClient("localhost",6334));
builder.Services.AddSingleton<IVectorStore,QdrantVectorStore>();

//register ingestion service
builder.Services.AddScoped<IngestionService>();

// register the ingestion worker
builder.Services.AddSingleton(Channel.CreateUnbounded<Guid>());
builder.Services.AddHostedService<IngestionWorker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
public partial class Program
{
}