using System;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;

namespace Remp.Remp.Repository;

public class CaseContactRepository : ICaseContactRepository
{
    private readonly RempDbContext _dbContext;

    public CaseContactRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CaseContact>> GetCaseContactsByListingCaseIdAsync(int listingCaseId)
    {
        return await _dbContext.CaseContacts
            .Where(c => c.ListingCaseId == listingCaseId)
            .ToListAsync();
    }
}
