using System;

namespace Remp.Remp.Models.DTOs;

public class CurrentUserResponseDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public List<int> AssignedListingCaseIds { get; set; }

    public string? PhotographyCompanyName { get; set; }

    public string? AgentFirstName { get; set; }
    public string? AgentLastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CompanyName { get; set; }
}
