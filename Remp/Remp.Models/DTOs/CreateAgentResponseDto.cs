using System;

namespace Remp.Remp.Models.DTOs;

public class CreateAgentResponseDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string AgentFirstName { get; set; }
    public string AgentLastName { get; set; }
    public string GeneratedPassword { get; set; }
}
