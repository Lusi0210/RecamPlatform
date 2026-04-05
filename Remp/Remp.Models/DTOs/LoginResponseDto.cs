using System;

namespace Remp.Remp.Models.DTOs;

public class LoginResponseDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }


    public string? PhotographyCompanyName { get; set; }

    public string? AgentFirstName { get; set; }
    public string? AgentLastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CompanyName { get; set; }
}
