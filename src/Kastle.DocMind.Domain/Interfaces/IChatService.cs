namespace Kastle.DocMind.Domain.Interfaces;
public interface IChatService
{
    Task<string>GetResponseAsync(string prompt,CancellationToken cancellationToken=default);
}