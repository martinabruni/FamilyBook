using Family.Book.Domain.Models;
using Family.Book.Domain.Validators;
using FluentValidation;

namespace Family.Book.Business.Validators;

public class AlbumValidator : BaseValidator<Family.Book.Domain.Models.Album>
{
    public AlbumValidator(Family.Book.Domain.Models.Album instance) : base(instance) { }

    public override void Validate(Family.Book.Domain.Models.Album instance)
    {
        var validator = new InlineValidator<Family.Book.Domain.Models.Album>();
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