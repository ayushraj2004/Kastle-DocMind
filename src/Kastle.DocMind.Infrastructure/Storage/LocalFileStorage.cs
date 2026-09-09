using Kastle.DocMind.Domain.Interfaces;
using Kastle.DocMind.Infrastructure.Data;
using Microsoft.Extensions.Options;

namespace Kastle.DocMind.Infrastructure.Storage;
public class LocalFileStorage : IFileStorage// here also adding DI 
{
    private readonly string _storagePath;
    public LocalFileStorage(IOptions<FileStorageSettings>settings)
    {
        _storagePath=Path.Combine(Directory.GetCurrentDirectory(),settings.Value.StoragePath); // gets application's current directory 
        Directory.CreateDirectory(_storagePath);//creates the folder automatically if it not exist
    }
    public async Task<string>SaveAsync(Stream fileStream,string fileName)
    {
        var uniqueFileName=$"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";// for reducing the overwrite we add Guid Id here so every file gets unique number
        var filePath=Path.Combine(_storagePath,uniqueFileName);
        await using var outputStream=new FileStream(filePath,FileMode.Create);
        await fileStream.CopyToAsync(outputStream);
        return filePath;
    }
    public Task DeleteAsync(string filePath)
    {
        if(File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

}