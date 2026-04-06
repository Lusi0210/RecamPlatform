using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class AgentPhotographyCompanyService : IAgentPhotographyCompanyService
{
    private readonly IAgentPhotographyCompanyRepository _agentPhotographyCompanyRepository;

    public AgentPhotographyCompanyService(IAgentPhotographyCompanyRepository agentPhotographyCompanyRepository)
    {
        _agentPhotographyCompanyRepository = agentPhotographyCompanyRepository;
    }

    public async Task<AgentPhotographyCompanyResponseDto> AddAgentToCompanyAsync(string agentId, string photographyCompanyId)
    {
        bool exists = await _agentPhotographyCompanyRepository.RelationExistsAsync(agentId, photographyCompanyId);
        if (exists)
        {
            throw new InvalidOperationException("This agent is already added to this photography company.");
        }

        AgentPhotographyCompany agentPhotographyCompany = new AgentPhotographyCompany
        {
            AgentId = agentId,
            PhotographyCompanyId = photographyCompanyId
        };

        AgentPhotographyCompany result = await _agentPhotographyCompanyRepository.AddAgentToCompanyAsync(agentPhotographyCompany);

        AgentPhotographyCompanyResponseDto responseDto = new AgentPhotographyCompanyResponseDto
        {
            AgentId = result.AgentId,
            PhotographyCompanyId = result.PhotographyCompanyId
        };

        return responseDto;
    }
}
