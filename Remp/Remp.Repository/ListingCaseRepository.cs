using System;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.Models.Enum;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Repository;

public class ListingCaseRepository: IListingCaseRepository
{
    private RempDbContext _dbContext;

    public ListingCaseRepository(RempDbContext rempDb)
    {
        _dbContext = rempDb;
    }
    public async Task<ListingCase> AddListingCaseAsync(ListingCase listingCase)
    {
        await _dbContext.ListingCases.AddAsync(listingCase);

        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return listingCase;
        }
        else
        {
            throw new Exception("Failed to add listing case.");
        }
    }

    public async Task<List<ListingCase>> GetAllListingCasesAsync()
    {
        return await _dbContext.ListingCases.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<ListingCase> GetListingCaseByIdAsync(int id)
    {
        ListingCase? listingCase = await _dbContext.ListingCases.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (listingCase != null)
        {
            return listingCase;
        }
        else
        {
            throw new KeyNotFoundException($"Listing case with ID {id} not found.");
        }
    }

    public async Task<ListingCase> UpdateListingCaseAsync(ListingCase listingCase)
    {
        _dbContext.ListingCases.Update(listingCase);
        int changes = await _dbContext.SaveChangesAsync();
        return listingCase;
    }

    public async Task<bool> DeleteListingCaseAsync(int id)
    {
        ListingCase? listingCase = await _dbContext.ListingCases.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        
        if (listingCase == null)
        {
            throw new Exception($"Listing case with ID {id} not found.");
        }
        listingCase.IsDeleted = true;
        _dbContext.ListingCases.Update(listingCase);
        int changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<ListingCase> UpdateListingCaseStatusAsync(int id, Models.Enum.ListcaseStatus newStatus)
    {
        ListingCase? listingCase = await _dbContext.ListingCases.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        
        if (listingCase == null)
        {
            throw new Exception($"Listing case with ID {id} not found.");
        }
        listingCase.ListcaseStatus = newStatus;
        _dbContext.ListingCases.Update(listingCase);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return listingCase;
        }
        else
        {
            throw new Exception($"Failed to update status for listing case with ID {id}.");
        }
    }
}
    