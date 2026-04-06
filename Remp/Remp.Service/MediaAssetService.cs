using System;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Enum;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class MediaAssetService : IMediaAssetService
{
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IBlobStorageService _blobStorageService;

    public MediaAssetService(IMediaAssetRepository mediaAssetRepository, IBlobStorageService blobStorageService)
    {
        _mediaAssetRepository = mediaAssetRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<List<GroupedMediaResponseDto>> GetMediaByListingCaseIdAsync(int listingCaseId)
    {
        List<MediaAsset> mediaAssets = await _mediaAssetRepository.GetMediaByListingCaseIdAsync(listingCaseId);

        List<GroupedMediaResponseDto> groupedMedia = mediaAssets
            .GroupBy(m => m.MediaType)
            .Select(g => new GroupedMediaResponseDto
            {
                MediaType = g.Key,
                MediaAssets = g.Select(m => new MediaAssetResponseDto
                {
                    Id = m.Id,
                    MediaType = m.MediaType,
                    MediaUrl = m.MediaUrl,
                    UploadedAt = m.UploadedAt,
                    IsSelect = m.IsSelect,
                    IsHero = m.IsHero,
                    ListingCaseId = m.ListingCaseId
                }).ToList()
            }).ToList();

        return groupedMedia;
    }

    public async Task<bool> DeleteMediaAsync(int mediaId, string userId)
    {
        MediaAsset mediaAsset = await _mediaAssetRepository.GetMediaByIdAsync(mediaId);

        if (mediaAsset.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this media asset.");
        }

        bool result = await _mediaAssetRepository.DeleteMediaAsync(mediaId);
        return result;
    }

    public async Task<List<MediaAssetResponseDto>> UploadMediaAssetsAsync(List<IFormFile> files, MediaType mediaType, int listingCaseId, string userId)
    {
        // Validate: non-picture types only allow single file
        if (mediaType != MediaType.Photos && files.Count > 1)
        {
            throw new InvalidOperationException("Only Photos type allows multiple file uploads.");
        }

        // Validate: at least one file
        if (files.Count < 1)
        {
            throw new InvalidOperationException("At least one file is required.");
        }

        List<MediaAssetResponseDto> responseDtos = new List<MediaAssetResponseDto>();

        foreach (IFormFile file in files)
        {
            // Upload to Blob Storage
            using Stream stream = file.OpenReadStream();
            string mediaUrl = await _blobStorageService.UploadFileAsync(stream, file.FileName, file.ContentType);

            // Save to database
            MediaAsset mediaAsset = new MediaAsset
            {
                MediaType = mediaType,
                MediaUrl = mediaUrl,
                UploadedAt = DateTime.UtcNow,
                IsSelect = false,
                IsHero = false,
                ListingCaseId = listingCaseId,
                UserId = userId,
                IsDeleted = false
            };

            MediaAsset savedAsset = await _mediaAssetRepository.AddMediaAssetAsync(mediaAsset);

            responseDtos.Add(new MediaAssetResponseDto
            {
                Id = savedAsset.Id,
                MediaType = savedAsset.MediaType,
                MediaUrl = savedAsset.MediaUrl,
                UploadedAt = savedAsset.UploadedAt,
                IsSelect = savedAsset.IsSelect,
                IsHero = savedAsset.IsHero,
                ListingCaseId = savedAsset.ListingCaseId
            });
        }

        return responseDtos;
    }

    public async Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId)
    {
        MediaAsset mediaAsset = await _mediaAssetRepository.GetMediaByIdAsync(mediaAssetId);

        var (fileStream, contentType, fileName) = await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);

        return (fileStream, contentType, fileName);
    }

    public async Task<(Stream Content, string FileName)> DownloadAllMediaByListingCaseAsync(int listingCaseId)
    {
        List<MediaAsset> mediaAssets = await _mediaAssetRepository.GetMediaByListingCaseIdAsync(listingCaseId);

        if (mediaAssets.Count == 0)
        {
            throw new KeyNotFoundException($"No media assets found for listing case {listingCaseId}.");
        }

        MemoryStream zipStream = new MemoryStream();

        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (MediaAsset mediaAsset in mediaAssets)
            {
                var (fileStream, contentType, fileName) = await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);

                ZipArchiveEntry entry = archive.CreateEntry(fileName);
                using Stream entryStream = entry.Open();
                await fileStream.CopyToAsync(entryStream);
            }
        }

        zipStream.Position = 0;
        string zipFileName = $"ListingCase_{listingCaseId}_Media.zip";

        return (zipStream, zipFileName);
    }
}
