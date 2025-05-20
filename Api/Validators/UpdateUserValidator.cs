using Api.Dtos;
using FluentValidation;

namespace Api.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Username != null || x.Email != null)
            .WithMessage("At least one of Username or Email must be provided.");
        
        When(x => x.Username != null, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(20);
        });

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(254)
                .EmailAddress();
        });
    }
}
