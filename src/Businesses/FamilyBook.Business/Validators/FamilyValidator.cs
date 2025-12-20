using FamilyBook.Domain.Models;
using FluentValidation;

namespace FamilyBook.Business.Validators;

public class FamilyValidator : BaseValidator<Family>
{
    public FamilyValidator(Family instance) : base(instance) { }

    public override void Validate(Family instance)
    {
        var validator = new InlineValidator<Family>();
        validator.RuleFor(f => f.Id)
            .NotEmpty();

        validator.RuleFor(f => f.Name)
            .NotEmpty()
            .Length(1, 120)
            .Must(name => !string.IsNullOrWhiteSpace(name.Trim()))
            .WithMessage("Name must not be empty or whitespace only.");

        validator.RuleFor(f => f.LastUpdatedDate)
            .GreaterThanOrEqualTo(f => f.CreatedDate);

        var result = validator.Validate(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}