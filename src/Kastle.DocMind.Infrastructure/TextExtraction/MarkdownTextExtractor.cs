using Kastle.DocMind.Domain.Interfaces;
using System.Text;

namespace Kastle.DocMind.Infrastructure.TextExtraction;
public class  MarkdownTextExtractor:ITextExtractor{  //extract the text from markdown file
    public async Task<string>ExtractTextAsync(Stream fileStream,string fileName)//read the text and return txt
{
    using var reader =new StreamReader(fileStream,Encoding.UTF8,detectEncodingFromByteOrderMarks:true,leaveOpen:true);//create reader to read the file
    return await reader.ReadToEndAsync();
}
    
}