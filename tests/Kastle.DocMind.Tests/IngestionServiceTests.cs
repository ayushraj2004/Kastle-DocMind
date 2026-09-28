using Kastle.DocMind.Business.Services;
using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Domain.Interfaces;
using Microsoft.Extensions.AI;
using Moq;
using Xunit;
namespace Kastle.DocMind.Test;
public class IngestionServiceTests
{
    [Fact]
    public async Task ProcessAsync_EmptyText_DoesNotStoreChunks()
    {
        var chunking=new Mock<IChunkingService>();
        var vectorStore=new Mock<IVectorStore>();
        chunking.
            Setup(x=>x.ChunkText(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Array.Empty<Chunk>());
        var embeddings=new Mock<
            IEmbeddingGenerator<string,Embedding<float>>>();
        var service=new IngestionService(chunking.Object,vectorStore.Object,embeddings.Object);
        await service.ProcessAsync(Guid.NewGuid(), "");
        vectorStore.Verify(
            x => x.UpsertAsync(
                It.IsAny<IReadOnlyList<Chunk>>(),
                It.IsAny<IReadOnlyList<float[]>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

    }
}