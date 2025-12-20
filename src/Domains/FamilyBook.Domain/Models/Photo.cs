using static FamilyBook.Domain.Enums;

namespace FamilyBook.Domain.Models;

/// <summary>
/// Represents a photo entity.
/// Rules:
/// - Id: required, Guid != Guid.Empty.
/// - MemberId: required, Guid != Guid.Empty.
/// - AlbumId: required, Guid != Guid.Empty.
/// - Location: optional, length 0..200, trim.
/// - Description: optional, length 0..2000, trim.
/// - PublicationDate: required, <= today + 5 minutes (for clock skew) and >= CreatedDate - 1 day.
/// - CreatedDate: required.
/// - LastUpdatedDate: required, >= CreatedDate.
/// - OriginalBlobKey: required, length 1..500.
/// - ThumbnailBlobKey: required, length 1..500.
/// - ContentType: required, whitelist image/jpeg, image/png, image/webp.
/// - SizeBytes: required, >0 and <= 25MB.
/// - Status: required.
/// </summary>
public sealed class Photo : BaseModel
{
    public Guid MemberId { get; }
    public Guid AlbumId { get; }
    public string? Location { get; }
    public string? Description { get; }
    public DateTime PublicationDate { get; }
    public DateTime CreatedDate { get; }
    public DateTime LastUpdatedDate { get; }
    public string OriginalBlobKey { get; }
    public string ThumbnailBlobKey { get; }
    public string ContentType { get; }
    public long SizeBytes { get; }
    public PhotoStatus Status { get; }

    public Photo(Guid id, Guid memberId, Guid albumId, string? location, string? description, DateTime publicationDate, DateTime createdDate, DateTime lastUpdatedDate, string originalBlobKey, string thumbnailBlobKey, string contentType, long sizeBytes, PhotoStatus status)
        : base(id)
    {
        MemberId = memberId;
        AlbumId = albumId;
        Location = location;
        Description = description;
        PublicationDate = publicationDate;
        CreatedDate = createdDate;
        LastUpdatedDate = lastUpdatedDate;
        OriginalBlobKey = originalBlobKey;
        ThumbnailBlobKey = thumbnailBlobKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Status = status;
    }
}