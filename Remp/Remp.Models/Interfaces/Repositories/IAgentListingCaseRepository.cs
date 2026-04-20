using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IAgentListingCaseRepository
{
    Task<bool> RelationExistsAsync(string agentId, int listingCaseId);
    Task<AgentListingCase> AddAgentToListingCaseAsync(AgentListingCase agentListingCase);
}
