using PhotoGallery.Functions.Domain.Models;

namespace PhotoGallery.Functions.Domain.Repositories;

public interface IPhotoRepository
{
    Task<List<string>> GetAlbumNamesAsync(CancellationToken cancellationToken = default);
    Task<List<Photo>> GetPhotosFromAlbumAsync(string albumName, CancellationToken cancellationToken = default);
}
