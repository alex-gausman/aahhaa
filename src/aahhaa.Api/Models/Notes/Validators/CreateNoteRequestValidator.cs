using FluentValidation;

namespace aahhaa.Api.Models.Notes.Validators;

public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(r => r.Text).MaximumLength(500);
        RuleFor(r => r.Title).NotEmpty().MaximumLength(50);
    }
}