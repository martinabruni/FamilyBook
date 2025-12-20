namespace FamilyBook.Domain.Models;

/// <summary>
/// Represents an album entity.
/// Rules:
/// - Id: required, Guid != Guid.Empty.
/// - FamilyId: required, Guid != Guid.Empty.
/// - Name: required, length 1..120, trim, no solo whitespace.
/// - CreatedDate: required.
/// - LastUpdatedDate: required, >= CreatedDate.
/// </summary>
public sealed class Album : BaseModel
{
    public Guid FamilyId { get; }
    public string Name { get; }
    public DateTimeOffset CreatedDate { get; }
    public DateTimeOffset LastUpdatedDate { get; }

    public Album(Guid id, Guid familyId, string name, DateTimeOffset createdDate, DateTimeOffset lastUpdatedDate)
        : base(id)
    {
        FamilyId = familyId;
        Name = name;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
    }
}