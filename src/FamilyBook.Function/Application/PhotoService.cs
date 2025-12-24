using Microsoft.Extensions.Configuration;
using PhotoGallery.Functions.Domain.Models;
using PhotoGallery.Functions.Domain.Repositories;
using PhotoGallery.Functions.Domain.Services;

namespace PhotoGallery.Functions.Application.Services;

internal sealed class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _photoRepository;
    private readonly string _baseUrl;

    public PhotoService(IPhotoRepository photoRepository, IConfiguration configuration)
    {
        _photoRepository = photoRepository;
        _baseUrl = configuration["AzureBlobStorage:BaseUrl"] 
            ?? throw new InvalidOperationException("AzureBlobStorage:BaseUrl is not configured");
    }

    public async Task<List<string>> GetAlbumNamesAsync(CancellationToken cancellationToken = default)
    {
        return await _photoRepository.GetAlbumNamesAsync(cancellationToken);
    }

    public async Task<Album> GetAlbumAsync(string albumName, CancellationToken cancellationToken = default)
    {
        var photos = await _photoRepository.GetPhotosFromAlbumAsync(albumName, cancellationToken);
        
        // Trova la cover image
        var coverImageUrl = $"{_baseUrl}/{albumName}/cover.jpg";

        return new Album
        {
            Id = albumName,
            Name = FormatAlbumName(albumName),
            Description = $"Album con {photos.Count} foto",
            CoverImageUrl = coverImageUrl,
            Photos = photos
        };
    }

    public async Task<GalleryConfig> GetGalleryConfigAsync(CancellationToken cancellationToken = default)
    {
        var albumNames = await GetAlbumNamesAsync(cancellationToken);
        var albums = new List<Album>();

        foreach (var albumName in albumNames)
        {
            var album = await GetAlbumAsync(albumName, cancellationToken);
            albums.Add(album);
        }

        return new GalleryConfig
        {
            BaseUrl = _baseUrl,
            Albums = albums
        };
    }

    private static string FormatAlbumName(string albumName)
    {
        // Converte "natale-2025" in "Natale 2025"
        return string.Join(' ', albumName.Split('-')
            .Select(word => char.ToUpper(word[0]) + word[1..]));
    }
}
