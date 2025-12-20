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
    public DateTime CreatedDate { get; }
    public DateTime LastUpdatedDate { get; }

    public Album(Guid id, Guid familyId, string name, DateTime createdDate, DateTime lastUpdatedDate)
        : base(id)
    {
        FamilyId = familyId;
        Name = name;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
    }
}