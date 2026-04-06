using System;

namespace Remp.Remp.Models.DTOs;

public class UpdatePasswordRequestDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
