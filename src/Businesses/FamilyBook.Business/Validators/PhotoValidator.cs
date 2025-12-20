using FamilyBook.Domain.Models;
using FamilyBook.Domain.Validators;
using FluentValidation;

namespace FamilyBook.Business.Validators;

public class PhotoValidator : BaseValidator<FamilyBook.Domain.Models.Photo>
{
    public PhotoValidator(FamilyBook.Domain.Models.Photo instance) : base(instance) { }

    public override void Validate(FamilyBook.Domain.Models.Photo instance)
    {
        var validator = new InlineValidator<FamilyBook.Domain.Models.Photo>();
        validator.RuleFor(p => p.Id)
            .NotEmpty();

        validator.RuleFor(p => p.MemberId)
            .NotEmpty();

        validator.RuleFor(p => p.AlbumId)
            .NotEmpty();

        validator.RuleFor(p => p.Location)
            .Length(0, 200)
            .When(p => p.Location != null);

        validator.RuleFor(p => p.Description)
            .Length(0, 2000)
            .When(p => p.Description != null);

        validator.RuleFor(p => p.PublicationDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .GreaterThanOrEqualTo(p => p.CreatedDate.AddDays(-1));

        validator.RuleFor(p => p.LastUpdatedDate)
            .GreaterThanOrEqualTo(p => p.CreatedDate);

        validator.RuleFor(p => p.OriginalBlobKey)
            .NotEmpty()
            .Length(1, 500);

        validator.RuleFor(p => p.ThumbnailBlobKey)
            .NotEmpty()
            .Length(1, 500);

        validator.RuleFor(p => p.ContentType)
            .NotEmpty()
            .Must(ct => new[] { "image/jpeg", "image/png", "image/webp" }.Contains(ct))
            .WithMessage("ContentType must be one of: image/jpeg, image/png, image/webp.");

        validator.RuleFor(p => p.SizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(25L * 1024 * 1024);

        validator.RuleFor(p => p.Status)
            .IsInEnum();

        var result = validator.Validate(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}