using FluentValidation;
using Kastle.DocMind.Api.Models;
namespace Kastle.DocMind.Api.Validators;
//upload doc and only two file acceptable 
public class UploadDocumentRequestValidator : AbstractValidator<UploadDocumentRequest>
{
    public UploadDocumentRequestValidator()
    {
        RuleFor(x=>x.File).NotNull().WithMessage("File is required");
        RuleFor(x=>x.File).Must(file=>file==null || file.Length>0).WithMessage("Fill can't be empty");
        RuleFor(x=>x.File).Must(file=>file==null || new[]{".txt",".md"}.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())).WithMessage("only txt and md file allowed");
    }
}