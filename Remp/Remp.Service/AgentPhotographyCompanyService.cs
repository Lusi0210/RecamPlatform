using System;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Remp.Remp.Service;

public class AgentPhotographyCompanyService : IAgentPhotographyCompanyService
{
    private readonly IAgentPhotographyCompanyRepository _agentPhotographyCompanyRepository;
    private readonly RempDbContext _dbContext;


    public AgentPhotographyCompanyService(IAgentPhotographyCompanyRepository agentPhotographyCompanyRepository, RempDbContext dbContext)
    {
        _agentPhotographyCompanyRepository = agentPhotographyCompanyRepository;
        _dbContext = dbContext;
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

    public async Task<List<UserResponseDto>> GetAgentsByCompanyAsync(string photographyCompanyId)
    {
        List<string> agentIds = await _agentPhotographyCompanyRepository.GetAgentIdsByCompanyIdAsync(photographyCompanyId);

        List<UserResponseDto> agents = await _dbContext.Agents
            .Where(a => agentIds.Contains(a.Id))
            .Join(_dbContext.Users,
                a => a.Id,
                u => u.Id,
                (a, u) => new UserResponseDto
                {
                    UserId = a.Id,
                    Email = u.Email,
                    Role = "Agent",
                    AgentFirstName = a.AgentFirstName,
                    AgentLastName = a.AgentLastName,
                    AvatarUrl = a.AvatarUrl,
                    CompanyName = a.CompanyName
                }).ToListAsync();

        return agents;
    }

}

