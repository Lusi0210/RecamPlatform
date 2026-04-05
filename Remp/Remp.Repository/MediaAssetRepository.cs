using System;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;

namespace Remp.Remp.Repository;

public class MediaAssetRepository : IMediaAssetRepository
{
    private readonly RempDbContext _dbContext;

    public MediaAssetRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MediaAsset>> GetMediaByListingCaseIdAsync(int listingCaseId)
    {
        return await _dbContext.MediaAssets
            .Where(m => m.ListingCaseId == listingCaseId && !m.IsDeleted)
            .OrderByDescending(m => m.UploadedAt)
            .ToListAsync();
    }
}
