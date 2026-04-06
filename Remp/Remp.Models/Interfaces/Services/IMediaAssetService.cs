using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Enum;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IMediaAssetService
{
    Task<List<GroupedMediaResponseDto>> GetMediaByListingCaseIdAsync(int listingCaseId);
    Task<bool> DeleteMediaAsync(int mediaId, string userId);
    Task<List<MediaAssetResponseDto>> UploadMediaAssetsAsync(List<IFormFile> files, MediaType mediaType, int listingCaseId, string userId);
    Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId);
    Task<(Stream Content, string FileName)> DownloadAllMediaByListingCaseAsync(int listingCaseId);
    Task<MediaAssetResponseDto> SetCoverImageAsync(int listingCaseId, int mediaId);
    Task<List<MediaAssetResponseDto>> GetFinalSelectionAsync(int listingCaseId);
}
