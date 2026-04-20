using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class AgentListingCaseService : IAgentListingCaseService
{
    private readonly IAgentListingCaseRepository _agentListingCaseRepository;

    public AgentListingCaseService(IAgentListingCaseRepository agentListingCaseRepository)
    {
        _agentListingCaseRepository = agentListingCaseRepository;
    }

    public async Task<AddAgentToListingCaseRequestDto> AddAgentToListingCaseAsync(string agentId, int listingCaseId)
    {
        bool exists = await _agentListingCaseRepository.RelationExistsAsync(agentId, listingCaseId);
        if (exists)
        {
            throw new InvalidOperationException("This agent is already assigned to this listing case.");
        }

        AgentListingCase agentListingCase = new AgentListingCase
        {
            AgentId = agentId,
            ListingCaseId = listingCaseId
        };

        await _agentListingCaseRepository.AddAgentToListingCaseAsync(agentListingCase);

        AddAgentToListingCaseRequestDto responseDto = new AddAgentToListingCaseRequestDto
        {
            AgentId = agentId,
            ListingCaseId = listingCaseId
        };

        return responseDto;
    }
}
