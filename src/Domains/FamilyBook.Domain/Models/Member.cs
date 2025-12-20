namespace FamilyBook.Domain.Models;

/// <summary>
/// Represents a family member entity.
/// Rules:
/// - Id: required, Guid != Guid.Empty.
/// - FamilyId: required, Guid != Guid.Empty.
/// - FirstName: required, length 1..120, trim, no solo whitespace.
/// - LastName: required, length 1..120, trim, no solo whitespace.
/// - BirthDate: required, <= today (UTC) and >= today - 130 years.
/// - CreatedDate: required.
/// - LastUpdatedDate: required, >= CreatedDate.
/// </summary>
public sealed class Member : BaseModel
{
    public Guid FamilyId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime BirthDate { get; }
    public DateTime CreatedDate { get; }
    public DateTime LastUpdatedDate { get; }

    public Member(Guid id, Guid familyId, string firstName, string lastName, DateTime birthDate, DateTime createdDate, DateTime lastUpdatedDate)
        : base(id)
    {
        FamilyId = familyId;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
    }
}