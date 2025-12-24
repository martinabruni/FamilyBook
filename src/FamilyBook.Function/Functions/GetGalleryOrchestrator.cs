using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using PhotoGallery.Functions.Domain.Models;

namespace PhotoGallery.Functions.Functions;

internal sealed class GetGalleryOrchestrator
{
    private readonly ILogger<GetGalleryOrchestrator> _logger;

    public GetGalleryOrchestrator(ILogger<GetGalleryOrchestrator> logger)
    {
        _logger = logger;
    }

    [Function(nameof(GetGalleryOrchestrator))]
    public async Task<GalleryConfig> RunAsync([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(_logger);
        
        logger.LogInformation("Starting gallery orchestration");

        // Step 1: Get album names
        var albumNames = await context.CallActivityAsync<List<string>>(
            nameof(Activities.GetAlbumsActivity),
            string.Empty);

        logger.LogInformation("Found {Count} albums, fetching details", albumNames.Count);

        // Step 2: Get details for each album in parallel
        var albumTasks = albumNames.Select(albumName =>
            context.CallActivityAsync<Album>(
                nameof(Activities.GetAlbumDetailsActivity),
                albumName)
        ).ToList();

        var albums = await Task.WhenAll(albumTasks);

        logger.LogInformation("Fetched details for all albums");

        // Step 3: Build final gallery config
        var galleryConfig = await context.CallActivityAsync<GalleryConfig>(
            nameof(Activities.BuildGalleryConfigActivity),
            albums.ToList());

        logger.LogInformation("Gallery orchestration completed successfully");

        return galleryConfig;
    }
}
