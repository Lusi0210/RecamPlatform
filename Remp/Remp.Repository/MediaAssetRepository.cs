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

    public async Task<MediaAsset> GetMediaByIdAsync(int id)
    {
        MediaAsset? mediaAsset = await _dbContext.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        if (mediaAsset == null)
        {
            throw new KeyNotFoundException($"Media asset with ID {id} not found.");
        }
        return mediaAsset;
    }

    public async Task<bool> DeleteMediaAsync(int id)
    {
        MediaAsset? mediaAsset = await _dbContext.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        if (mediaAsset == null)
        {
            throw new KeyNotFoundException($"Media asset with ID {id} not found.");
        }
        mediaAsset.IsDeleted = true;
        _dbContext.MediaAssets.Update(mediaAsset);
        int changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<MediaAsset> AddMediaAssetAsync(MediaAsset mediaAsset)
    {
        await _dbContext.MediaAssets.AddAsync(mediaAsset);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return mediaAsset;
        }
        throw new Exception("Failed to add media asset.");
    }

    public async Task<MediaAsset> UpdateMediaAssetAsync(MediaAsset mediaAsset)
    {
        _dbContext.MediaAssets.Update(mediaAsset);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return mediaAsset;
        }
        throw new Exception("Failed to update media asset.");
    }
}
