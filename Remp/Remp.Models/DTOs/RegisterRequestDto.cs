using System;

namespace Remp.Remp.Models.DTOs;

public class RegisterRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string AgentFirstName { get; set; }
    public string AgentLastName { get; set; }
}
