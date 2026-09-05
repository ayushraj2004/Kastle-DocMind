    using Kastle.DocMind.Domain.Interfaces;

    namespace Kastle.DocMind.Infrastructure.TextExtraction;
    public class TextExtractorResolver//decide extractor which file should based on file type
    {
        private readonly TxtTextExtractor _txtExtractor;
        private readonly MarkdownTextExtractor _markdownExtractor;
        public TextExtractorResolver(TxtTextExtractor txtExtractor,MarkdownTextExtractor markdownExtractor)//constructor receive both extractor help of DI
        {
            _txtExtractor=txtExtractor;
            _markdownExtractor=markdownExtractor;
        }
        public ITextExtractor Resolve(string fileName)
        {
            var extension=Path.GetExtension(fileName).ToLowerInvariant();//convert into the lowercase
            return extension switch
            {
                ".txt"=>_txtExtractor,
                ".md"=>_markdownExtractor,
                _ => throw new NotSupportedException($"File type'{extension}' not supported")// throw error if file neither txt nor md
            };
        }
        
    }