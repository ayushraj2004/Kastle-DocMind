using Kastle.DocMind.Domain.Entities;
using System.Text;
namespace Kastle.DocMind.Business.Services;
public class PromptBuilder
{
    public string Build(string question, IReadOnlyList<VectorSearchResult> chunks, IReadOnlyList<ConversationTurn>? history = null)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("You're DocMind, an internal document assistant.");
        prompt.AppendLine();
        prompt.AppendLine("Answer the question using only the provided documents.");
        prompt.AppendLine("Do not use knowledge outside the documents.");
        prompt.AppendLine("If the document doesn't contain the answer, say: \"The document doesn't cover this question\".");
        prompt.AppendLine("For every factual statement include a source marker such as [1], [2].");
        prompt.AppendLine();
        prompt.AppendLine("DOCUMENTATION:");
        prompt.AppendLine();
        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            prompt.AppendLine($"[{i + 1}] {chunk.FileName ?? "Unknown File"}");
            if (!string.IsNullOrWhiteSpace(chunk.Section))
                prompt.AppendLine($"Section: {chunk.Section}");
            prompt.AppendLine(chunk.Text);
            prompt.AppendLine();
        }
        if (history != null && history.Count > 0)
        {
            prompt.AppendLine("CONVERSATION HISTORY:");
            foreach (var turn in history)
            {
                prompt.AppendLine($"User: {turn.UserMessage}");
                prompt.AppendLine($"Assistant: {turn.AssistantMessage}");
            }
            prompt.AppendLine();
        }
        prompt.AppendLine("USER QUESTION:");
        prompt.AppendLine(question);
        return prompt.ToString();
    }
}