using Kastle.DocMind.Domain.Entities;
using System.Text;
namespace Kastle.DocMind.Business.Services;
public class PromptBuilder
{
    public string Build(string question, IReadOnlyList<VectorSearchResult> chunks)
    {
        var prompt=new StringBuilder();
        prompt.AppendLine("You'r DocMind, an internal document assistant");
        prompt.AppendLine();
        prompt.AppendLine("Answer the question using only provided document");
        prompt.AppendLine("Don't outside the document to answer the question");
        prompt.AppendLine("if the document doesn't contain the answer, say:");
        prompt.AppendLine("\",the document doesn't cover this question\"");
        prompt.AppendLine("For every factual answer, include source maker such as[1],[2]");
        prompt.AppendLine();
        prompt.AppendLine("DOCUMENTATION:");
        prompt.AppendLine();
        for(int i = 0; i < chunks.Count; i++)
        {
            var chunk=chunks[i];
            prompt.AppendLine($"[{i+1}]{chunk.FileName ?? "Unknown File"}");
            if (!string.IsNullOrWhiteSpace(chunk.Section))
            {
                prompt.AppendLine($"Section: {chunk.Section}");

            }
            prompt.AppendLine(chunk.Text);
            prompt.AppendLine();
        }
        prompt.AppendLine("USER QUESTION:");
        prompt.AppendLine(question);
        return prompt.ToString();
    }
}