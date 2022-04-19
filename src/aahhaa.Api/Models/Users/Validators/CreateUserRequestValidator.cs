using FluentValidation;

namespace aahhaa.Api.Models.Users.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(u => u.Email).EmailAddress().MaximumLength(500);
            RuleFor(u => u.UserName).NotEmpty().MaximumLength(50);
            RuleFor(u => u.FirstName).NotEmpty().MaximumLength(200);
            RuleFor(u => u.LastName).NotEmpty().MaximumLength(200);
        }
    }
}
