using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IAgentListingCaseService
{
    Task<AddAgentToListingCaseRequestDto> AddAgentToListingCaseAsync(string agentId, int listingCaseId);
}
