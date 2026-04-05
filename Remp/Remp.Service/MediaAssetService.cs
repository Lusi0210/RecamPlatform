using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class MediaAssetService : IMediaAssetService
{
    private readonly IMediaAssetRepository _mediaAssetRepository;

    public MediaAssetService(IMediaAssetRepository mediaAssetRepository)
    {
        _mediaAssetRepository = mediaAssetRepository;
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
}
