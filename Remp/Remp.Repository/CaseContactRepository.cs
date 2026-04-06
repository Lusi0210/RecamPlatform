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

    public async Task<CaseContact> AddCaseContactAsync(CaseContact caseContact)
    {
        await _dbContext.CaseContacts.AddAsync(caseContact);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return caseContact;
        }
        throw new Exception("Failed to add case contact.");
    }
}
