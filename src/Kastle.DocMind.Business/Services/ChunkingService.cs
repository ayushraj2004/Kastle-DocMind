using Kastle.DocMind.Business.Options;
using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Domain.Entities;
using Microsoft.Extensions.Options;
namespace Kastle.DocMind.Business.Services;
public class ChunkingService : IChunkingService
{

    private readonly ChunkingOptions _options;
    public ChunkingService(IOptions<ChunkingOptions> options)
    {
        _options=options.Value;
    }
    public IReadOnlyList<Chunk>ChunkText(Guid documentId,string text,string? fileName=null,string? section = null)
    {
        //chunking logic here
        if (string.IsNullOrWhiteSpace(text))
        {
            return[];
        }
        if (_options.ChunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(_options.ChunkSize),"Chunk size have to greater that 0");
        }
        if(_options.Overlap<0 || _options.Overlap >= _options.ChunkSize)
        {
            throw new ArgumentOutOfRangeException(nameof(_options.Overlap),"overlap must zero or great or small than chunksize");
        }
        int chunkSize=_options.ChunkSize*4;
        int overlap=_options.Overlap*4;
        var chunks=new List<Chunk>();
        int start=0;
        int sequenceNumber=1;
        while (start < text.Length)
        {
            int length=Math.Min(chunkSize, text.Length - start);
            string chunkText=text.Substring(start, length);
            chunks.Add(new Chunk
            {
                Id=Guid.NewGuid(),
                DocumentId=documentId,
                SequenceNumber=sequenceNumber,
                Text=chunkText,
                FileName=fileName,
                TokenCount=chunkText.Length/4
            });
            sequenceNumber++;
            if (start + length >= text.Length)
            {
                break;
            }
            start += chunkSize-overlap;
        }
        return chunks;
    }
}