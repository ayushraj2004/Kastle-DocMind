    using Kastle.DocMind.Domain.Interfaces;

    namespace Kastle.DocMind.Infrastructure.TextExtraction;
    public class TextExtractorResolver : ITextExtractorResolver
{
    private readonly IEnumerable<ITextExtractor>_extractors;
    public TextExtractorResolver(IEnumerable<ITextExtractor> extractors)
    {
        _extractors=extractors;
    }
    public ITextExtractor Resolve(string fileName)
    {
        var extension=Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".txt"=>_extractors.OfType<TxtTextExtractor>().First(),
            ".md"=> _extractors.OfType<MarkdownTextExtractor>().First(),
            _ => throw new NotSupportedException($"file type'{extension}'not supported")
        };
    }
}