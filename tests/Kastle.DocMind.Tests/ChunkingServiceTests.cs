using Kastle.DocMind.Business.Options;
using Kastle.DocMind.Business.Services;
using Microsoft.Extensions.Options;
using Xunit;
namespace Kastle.DocMind.Tests;
public class ChunkingServiceTests
{
    [Fact]
    public void ChunkText_EmptyText_ReturnsNoChunks()
    {
        var options=Options.Create(new ChunkingOptions());
        var service=new ChunkingService(options);
        var result=service.ChunkText(Guid.NewGuid(), "");
        Assert.Empty(result);
    }
    [Fact]
    public void ChunkText_ShortText_ReturnsOneChunk()
    {
        var options=Options.Create(new ChunkingOptions());
        var service=new ChunkingService(options);
        var result=service.ChunkText(Guid.NewGuid(),"hiDocmind");
        Assert.Single(result);
        Assert.Equal("hiDocmind",result[0].Text);
    }
    [Fact]
    public void ChunkText_ChunksHaveSequentialNumbers()
    {
        var options=Options.Create(new ChunkingOptions{ChunkSize=5,Overlap=1});
        var service=new ChunkingService(options);
        var result=service.ChunkText(Guid.NewGuid(),new string('A',60));
        Assert.True(result.Count>1);
        Assert.Equal(Enumerable.Range(1,result.Count),result.Select(chunk=>chunk.SequenceNumber));
    }
}