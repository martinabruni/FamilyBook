namespace PhotoGallery.Functions.Domain.Models;

public sealed class Album
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CoverImageUrl { get; init; }
    public List<Photo> Photos { get; init; } = [];
    public int PhotoCount => Photos.Count;
}
