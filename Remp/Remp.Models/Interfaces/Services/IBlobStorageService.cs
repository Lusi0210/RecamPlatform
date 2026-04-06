using System;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl);
    Task DeleteFileAsync(string blobUrl);
}
