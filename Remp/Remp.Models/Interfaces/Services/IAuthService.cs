using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
}
