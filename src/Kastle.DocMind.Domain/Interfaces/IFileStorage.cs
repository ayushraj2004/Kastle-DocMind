namespace Kastle.DocMind.Domain.Entities;

/// <summary>
/// Represents a document entity in the system. 
/// </summary>
public interface IFileStorage
{
    Task<string> SaveAsync(Stream fileStream, string fileName);

    Task DeleteAsync(string filePath);
}