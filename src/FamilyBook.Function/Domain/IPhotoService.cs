using PhotoGallery.Functions.Domain.Models;

namespace PhotoGallery.Functions.Domain.Services;

public interface IPhotoService
{
    Task<List<string>> GetAlbumNamesAsync(CancellationToken cancellationToken = default);
    Task<Album> GetAlbumAsync(string albumName, CancellationToken cancellationToken = default);
    Task<GalleryConfig> GetGalleryConfigAsync(CancellationToken cancellationToken = default);
}
