using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        string connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
        string containerName = configuration["AzureBlobStorage:ContainerName"]!;
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        string blobName = $"{Guid.NewGuid()}_{fileName}";
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        BlobHttpHeaders headers = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = headers
        });

        return blobClient.Uri.ToString();
    }

    public async Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl)
    {
        string blobName = new Uri(blobUrl).Segments.Last();
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        BlobDownloadInfo download = await blobClient.DownloadAsync();

        return (download.Content, download.ContentType, blobName);
    }

    public async Task DeleteFileAsync(string blobUrl)
    {
        string blobName = new Uri(blobUrl).Segments.Last();
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }
}
