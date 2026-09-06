namespace Kastle.DocMind.Domain.Interfaces;
public interface ITextExtractorResolver
{
    ITextExtractor Resolve(string fileName);//give filename and give correct txt extractor
}