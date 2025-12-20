using FamilyBook.Domain.Models;
using FluentValidation;

namespace FamilyBook.Business.Validators;

public class AlbumValidator : BaseValidator<Album>
{
    public AlbumValidator(Album instance) : base(instance) { }

    public override void Validate(Album instance)
    {
        var validator = new InlineValidator<Album>();
        validator.RuleFor(a => a.Id)
            .NotEmpty();

        validator.RuleFor(a => a.FamilyId)
            .NotEmpty();

        validator.RuleFor(a => a.Name)
            .NotEmpty()
            .Length(1, 120)
            .Must(name => !string.IsNullOrWhiteSpace(name.Trim()))
            .WithMessage("Name must not be empty or whitespace only.");

        validator.RuleFor(a => a.LastUpdatedDate)
            .GreaterThanOrEqualTo(a => a.CreatedDate);

        var result = validator.Validate(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}