using Api.Dtos;
using FluentValidation;

namespace Api.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
