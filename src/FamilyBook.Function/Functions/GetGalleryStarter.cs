using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace PhotoGallery.Functions.Functions;

internal sealed class GetGalleryStarter
{
    private readonly ILogger<GetGalleryStarter> _logger;

    public GetGalleryStarter(ILogger<GetGalleryStarter> logger)
    {
        _logger = logger;
    }

    [Function(nameof(GetGalleryStarter))]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "gallery")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        _logger.LogInformation("HTTP trigger received request to get gallery");

        // Start the orchestration
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(GetGalleryOrchestrator));

        _logger.LogInformation("Started orchestration with ID = '{InstanceId}'", instanceId);

        // Return the status query response
        var response = await client.CreateCheckStatusResponseAsync(req, instanceId);

        return response;
    }
}
