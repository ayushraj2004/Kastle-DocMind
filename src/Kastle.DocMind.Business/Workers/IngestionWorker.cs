using System.Threading.Channels;
using Kastle.DocMind.Business.Services;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Kastle.DocMind.Business.Workers;



public class IngestionWorker : BackgroundService
{
    private readonly Channel<Guid> _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public IngestionWorker(
        Channel<Guid> channel,
        IServiceScopeFactory scopeFactory)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await foreach (var documentId in
            _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repository =
                    scope.ServiceProvider
                        .GetRequiredService<IDocumentRepository>();

                var ingestionService =
                    scope.ServiceProvider
                        .GetRequiredService<IngestionService>();

                var document =
                    await repository.GetByIdAsync(documentId);

                if (document == null)
                {
                    continue;
                }

                document.Status = "Processing";

                await repository.UpdateAsync(document);

                await using var stream = File.OpenRead(document.FilePath);
                var extractor = scope.ServiceProvider
                    .GetRequiredService<ITextExtractorResolver>();
                var text = await extractor.Resolve(document.FileName)
                    .ExtractTextAsync(stream, document.FileName);

                await ingestionService.ProcessAsync(
                    document.Id,
                    text,
                    document.FileName,
                    stoppingToken);

                document.Status = "Indexed";

                await repository.UpdateAsync(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ingestion failed for document {documentId}: {ex}");
            }
        }
    }
}