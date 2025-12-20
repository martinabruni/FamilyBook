using Family.Book.Domain.Models;
using Family.Book.Domain.Validators;
using FluentValidation;

namespace Family.Book.Business.Validators;

public class FamilyValidator : BaseValidator<Family.Book.Domain.Models.Family>
{
    public FamilyValidator(Family.Book.Domain.Models.Family instance) : base(instance) { }

    public override void Validate(Family.Book.Domain.Models.Family instance)
    {
        var validator = new InlineValidator<Family.Book.Domain.Models.Family>();
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