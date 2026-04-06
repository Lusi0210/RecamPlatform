using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IAgentPhotographyCompanyRepository
{
    Task<bool> RelationExistsAsync(string agentId, string photographyCompanyId);
    Task<AgentPhotographyCompany> AddAgentToCompanyAsync(AgentPhotographyCompany agentPhotographyCompany);
}
