using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<RegisterResponseDto> RegisterAgentAsync(RegisterRequestDto registerRequestDto);
    Task<PaginatedResponseDto<UserResponseDto>> GetAllUsersAsync(PaginationRequestDto paginationRequest);
    Task<CurrentUserResponseDto> GetCurrentUserAsync(string userId);
    Task<CreateAgentResponseDto> CreateAgentAsync(CreateAgentRequestDto requestDto, string photographyCompanyId);
    Task<UserResponseDto> SearchAgentByEmailAsync(string email);

}
