using System;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Remp.Remp.Repository;

public class AgentListingCaseRepository : IAgentListingCaseRepository
{
    private readonly RempDbContext _dbContext;

    public AgentListingCaseRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> RelationExistsAsync(string agentId, int listingCaseId)
    {
        return await _dbContext.AgentListingCases.AnyAsync(al => al.AgentId == agentId && al.ListingCaseId == listingCaseId);
    }

    public async Task<AgentListingCase> AddAgentToListingCaseAsync(AgentListingCase agentListingCase)
    {
        await _dbContext.AgentListingCases.AddAsync(agentListingCase);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return agentListingCase;
        }
        throw new Exception("Failed to add agent to listing case.");
    }
}
