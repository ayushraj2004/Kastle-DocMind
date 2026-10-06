using Kastle.DocMind.Domain.Entities;
using Kastle.DocMind.Business.Options;
using Microsoft.Extensions.Options;
namespace Kastle.DocMind.Business.Services;
public class ContextBudgeter
{
    private readonly ContextBudgetOptions _options;
    public ContextBudgeter(IOptions<ContextBudgetOptions>options)
    {
        _options=options.Value;
    }
    public IReadOnlyList<VectorSearchResult>LimitChunks(IReadOnlyList<VectorSearchResult>chunks)
    {
        var selected=new List<VectorSearchResult>();
        var currentCharacters=0;
        foreach(var chunk in chunks)
        {
            if (currentCharacters  + chunk.Text.Length > _options.MaxCharacters)
            {
                break;
            }
            selected.Add(chunk);
            currentCharacters+=chunk.Text.Length;
        }
        return selected;
    }
}