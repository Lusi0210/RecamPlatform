using System;

namespace Remp.Remp.Models.DTOs;

public class RegisterResponseDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string AgentFirstName { get; set; }
    public string AgentLastName { get; set; }
}
