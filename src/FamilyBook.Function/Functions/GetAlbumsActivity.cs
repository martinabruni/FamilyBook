using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using PhotoGallery.Functions.Domain.Services;

namespace PhotoGallery.Functions.Functions.Activities;

internal sealed class GetAlbumsActivity
{
    private readonly IPhotoService _photoService;
    private readonly ILogger<GetAlbumsActivity> _logger;

    public GetAlbumsActivity(IPhotoService photoService, ILogger<GetAlbumsActivity> logger)
    {
        _photoService = photoService;
        _logger = logger;
    }

    [Function(nameof(GetAlbumsActivity))]
    public async Task<List<string>> RunAsync([ActivityTrigger] string input)
    {
        _logger.LogInformation("Getting album names from blob storage");
        
        var albumNames = await _photoService.GetAlbumNamesAsync();
        
        _logger.LogInformation("Found {Count} albums", albumNames.Count);
        
        return albumNames;
    }
}
