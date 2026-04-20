using System;

namespace Remp.Remp.Models.DTOs;

public class AddAgentToListingCaseRequestDto
{
    public string AgentId { get; set; }
    public int ListingCaseId { get; set; }
}
