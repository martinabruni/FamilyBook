using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using PhotoGallery.Functions.Domain.Models;
using PhotoGallery.Functions.Domain.Repositories;

namespace PhotoGallery.Functions.Infrastructure.Repositories;

internal sealed class PhotoRepository : IPhotoRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string _baseUrl;

    public PhotoRepository(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"] 
            ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString is not configured");
        
        _containerName = configuration["AzureBlobStorage:ContainerName"] 
            ?? throw new InvalidOperationException("AzureBlobStorage:ContainerName is not configured");
        
        _baseUrl = configuration["AzureBlobStorage:BaseUrl"] 
            ?? throw new InvalidOperationException("AzureBlobStorage:BaseUrl is not configured");

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<List<string>> GetAlbumNamesAsync(CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var albumNames = new HashSet<string>();

        await foreach (var blobItem in containerClient.GetBlobsByHierarchyAsync(
            delimiter: "/",
            cancellationToken: cancellationToken))
        {
            if (blobItem.IsPrefix && blobItem.Prefix != null)
            {
                // Rimuove lo slash finale
                var albumName = blobItem.Prefix.TrimEnd('/');
                albumNames.Add(albumName);
            }
        }

        return [.. albumNames.OrderBy(x => x)];
    }

    public async Task<List<Photo>> GetPhotosFromAlbumAsync(string albumName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var photos = new List<Photo>();

        await foreach (var blobItem in containerClient.GetBlobsAsync(
            prefix: $"{albumName}/",
            cancellationToken: cancellationToken))
        {
            // Salta la cover image e file non immagine
            if (IsImageFile(blobItem.Name) && !IsCoverImage(blobItem.Name))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                
                photos.Add(new Photo
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = Path.GetFileName(blobItem.Name),
                    Url = blobClient.Uri.ToString(),
                    Alt = Path.GetFileNameWithoutExtension(blobItem.Name),
                    CreatedAt = blobItem.Properties.CreatedOn?.DateTime ?? DateTime.UtcNow,
                    SizeBytes = blobItem.Properties.ContentLength ?? 0
                });
            }
        }

        return photos.OrderBy(p => p.FileName).ToList();
    }

    private static bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp";
    }

    private static bool IsCoverImage(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant();
        return name == "cover";
    }
}
