using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using PhotoGallery.Functions.Domain.Models;
using PhotoGallery.Functions.Domain.Services;

namespace PhotoGallery.Functions.Functions.Activities;

internal sealed class GetAlbumDetailsActivity
{
    private readonly IPhotoService _photoService;
    private readonly ILogger<GetAlbumDetailsActivity> _logger;

    public GetAlbumDetailsActivity(IPhotoService photoService, ILogger<GetAlbumDetailsActivity> logger)
    {
        _photoService = photoService;
        _logger = logger;
    }

    [Function(nameof(GetAlbumDetailsActivity))]
    public async Task<Album> RunAsync([ActivityTrigger] string albumName)
    {
        _logger.LogInformation("Getting details for album: {AlbumName}", albumName);
        
        var album = await _photoService.GetAlbumAsync(albumName);
        
        _logger.LogInformation("Album {AlbumName} has {PhotoCount} photos", albumName, album.PhotoCount);
        
        return album;
    }
}
