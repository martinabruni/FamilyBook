using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoGallery.Functions.Domain.Models;

namespace PhotoGallery.Functions.Functions.Activities;

internal sealed class BuildGalleryConfigActivity
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BuildGalleryConfigActivity> _logger;

    public BuildGalleryConfigActivity(IConfiguration configuration, ILogger<BuildGalleryConfigActivity> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [Function(nameof(BuildGalleryConfigActivity))]
    public Task<GalleryConfig> RunAsync([ActivityTrigger] List<Album> albums)
    {
        _logger.LogInformation("Building gallery config with {Count} albums", albums.Count);
        
        var baseUrl = _configuration["AzureBlobStorage:BaseUrl"] 
            ?? throw new InvalidOperationException("AzureBlobStorage:BaseUrl is not configured");

        var config = new GalleryConfig
        {
            BaseUrl = baseUrl,
            Albums = albums
        };
        
        _logger.LogInformation("Gallery config built successfully");
        
        return Task.FromResult(config);
    }
}
