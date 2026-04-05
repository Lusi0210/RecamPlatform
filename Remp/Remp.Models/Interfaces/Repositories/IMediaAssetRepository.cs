using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IMediaAssetRepository
{
     Task<List<MediaAsset>> GetMediaByListingCaseIdAsync(int listingCaseId);
}
