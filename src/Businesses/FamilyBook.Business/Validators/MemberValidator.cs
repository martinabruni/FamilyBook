using FamilyBook.Domain.Models;
using FluentValidation;

namespace FamilyBook.Business.Validators;

public class MemberValidator : BaseValidator<Member>
{
    public MemberValidator(Member instance) : base(instance) { }

    public override void Validate(Member instance)
    {
        var validator = new InlineValidator<Member>();
        validator.RuleFor(m => m.Id)
            .NotEmpty();

        validator.RuleFor(m => m.FamilyId)
            .NotEmpty();

        validator.RuleFor(m => m.FirstName)
            .NotEmpty()
            .Length(1, 120)
            .Must(name => !string.IsNullOrWhiteSpace(name.Trim()))
            .WithMessage("FirstName must not be empty or whitespace only.");

        validator.RuleFor(m => m.LastName)
            .NotEmpty()
            .Length(1, 120)
            .Must(name => !string.IsNullOrWhiteSpace(name.Trim()))
            .WithMessage("LastName must not be empty or whitespace only.");

        validator.RuleFor(m => m.BirthDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-130));

        validator.RuleFor(m => m.LastUpdatedDate)
            .GreaterThanOrEqualTo(m => m.CreatedDate);

        var result = validator.Validate(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}