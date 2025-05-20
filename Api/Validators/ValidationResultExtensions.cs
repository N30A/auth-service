using FluentValidation.Results;
using Api.Dtos;

namespace Api.Validators;

public static class ValidationResultExtensions
{
    public static List<ValidationErrorDto> ToValidationErrorDtos(this ValidationResult result)
    {
        return result.Errors
            .Select(e => new ValidationErrorDto 
            { 
                Field = e.PropertyName, 
                Message = e.ErrorMessage 
            })
            .ToList();
    }
}
