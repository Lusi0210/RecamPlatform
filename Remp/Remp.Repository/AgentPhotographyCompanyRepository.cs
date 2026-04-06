using System;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;

namespace Remp.Remp.Repository;

public class AgentPhotographyCompanyRepository : IAgentPhotographyCompanyRepository
{
    private readonly RempDbContext _dbContext;

    public AgentPhotographyCompanyRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> RelationExistsAsync(string agentId, string photographyCompanyId)
    {
        return await _dbContext.AgentPhotographyCompanies
            .AnyAsync(ap => ap.AgentId == agentId && ap.PhotographyCompanyId == photographyCompanyId);
    }

    public async Task<AgentPhotographyCompany> AddAgentToCompanyAsync(AgentPhotographyCompany agentPhotographyCompany)
    {
        await _dbContext.AgentPhotographyCompanies.AddAsync(agentPhotographyCompany);
        int changes = await _dbContext.SaveChangesAsync();
        if (changes > 0)
        {
            return agentPhotographyCompany;
        }
        throw new Exception("Failed to add agent to photography company.");
    }
}
