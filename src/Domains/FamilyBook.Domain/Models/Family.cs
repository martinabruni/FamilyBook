namespace FamilyBook.Domain.Models;

/// <summary>
/// Represents a family entity.
/// Rules:
/// - Id: required, Guid != Guid.Empty.
/// - Name: required, length 1..120, trim, no solo whitespace.
/// - CreatedDate: required.
/// - LastUpdatedDate: required, >= CreatedDate.
/// </summary>
public sealed class Family : BaseModel
{
    public string Name { get; }
    public DateTime CreatedDate { get; }
    public DateTime LastUpdatedDate { get; }

    public Family(Guid id, string name, DateTime createdDate, DateTime lastUpdatedDate)
        : base(id)
    {
        Name = name;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
    }
}