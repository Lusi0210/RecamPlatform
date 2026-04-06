using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IAgentPhotographyCompanyService
{
    Task<AgentPhotographyCompanyResponseDto> AddAgentToCompanyAsync(string agentId, string photographyCompanyId);
}
