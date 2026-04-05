using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IMediaAssetService
{
    Task<List<GroupedMediaResponseDto>> GetMediaByListingCaseIdAsync(int listingCaseId);
}
