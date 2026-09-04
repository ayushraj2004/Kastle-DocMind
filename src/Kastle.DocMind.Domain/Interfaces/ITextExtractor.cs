namespace Kastle.DocMind.Domain.Interfaces;


// <summary>
/// Defines the contract for a text extractor service.
/// </summary>
public interface ITextExtractor
{
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
}