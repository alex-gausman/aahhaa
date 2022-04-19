using FluentValidation;

namespace aahhaa.Api.Models.Notes.Validators;

public class UpdateNoteRequestValidator : AbstractValidator<UpdateNoteRequest>
{
    public UpdateNoteRequestValidator()
    {
        RuleFor(r => r.Text).MaximumLength(500);
        RuleFor(r => r.Title).NotEmpty().MaximumLength(50);
    }
}