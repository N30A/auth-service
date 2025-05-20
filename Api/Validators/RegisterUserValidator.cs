using Api.Dtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Api.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();

        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
        {
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .Must(ContainsUppercase).WithMessage("Password must contain at least one uppercase letter.")
                .Must(ContainsDigit).WithMessage("Password must contain at least one digit.")
                .Must(ContainsSpecialCharacter).WithMessage("Password must contain at least one special character.");
        });

    }

    private static bool ContainsUppercase(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);

    private static bool ContainsDigit(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);

    private static bool ContainsSpecialCharacter(string password) =>
        !string.IsNullOrEmpty(password) && Regex.IsMatch(password, @"[\W]");
}