using Kastle.DocMind.Domain.Entities;
namespace Kastle.DocMind.Business.Services;
public class ContextBudgeter
{
    public IReadOnlyList<VectorSearchResult>LimitChunks(IReadOnlyList<VectorSearchResult>chunks,int maxCharacters)
    {
        var selected=new List<VectorSearchResult>();
        var currentCharacters=0;
        foreach(var chunk in chunks)
        {
            if (currentCharacters  + chunk.Text.Length > maxCharacters)
            {
                break;
            }
            selected.Add(chunk);
            currentCharacters+=chunk.Text.Length;
        }
        return selected;
    }
}