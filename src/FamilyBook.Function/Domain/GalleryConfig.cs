namespace PhotoGallery.Functions.Domain.Models;

public sealed class GalleryConfig
{
    public required string BaseUrl { get; init; }
    public List<Album> Albums { get; init; } = [];
}
