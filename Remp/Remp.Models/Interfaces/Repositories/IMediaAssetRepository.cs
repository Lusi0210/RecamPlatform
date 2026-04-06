using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IMediaAssetRepository
{
    Task<List<MediaAsset>> GetMediaByListingCaseIdAsync(int listingCaseId);
    Task<MediaAsset> GetMediaByIdAsync(int id);
    Task<bool> DeleteMediaAsync(int id);
    Task<MediaAsset> AddMediaAssetAsync(MediaAsset mediaAsset);
    Task<MediaAsset> UpdateMediaAssetAsync(MediaAsset mediaAsset);
    Task<List<MediaAsset>> GetSelectedMediaByListingCaseIdAsync(int listingCaseId);
}
