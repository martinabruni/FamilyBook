namespace PhotoGallery.Functions.Domain.Models;

public sealed class Photo
{
    public required string Id { get; init; }
    public required string FileName { get; init; }
    public required string Url { get; init; }
    public string? Alt { get; init; }
    public DateTime CreatedAt { get; init; }
    public long SizeBytes { get; init; }
}
