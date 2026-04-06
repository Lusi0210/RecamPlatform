using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Remp.Common.Responses;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            _logger.LogInformation("Login: Received login request");

            LoginResponseDto loginResponse = await _authService.LoginAsync(loginRequestDto);

            APIResponses<LoginResponseDto> apiResponse = new APIResponses<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = loginResponse,
                Errors = null
            };

            _logger.LogInformation($"Login: User {loginResponse.Email} logged in successfully with role {loginResponse.Role}");
            return Ok(apiResponse);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            _logger.LogInformation("Register: Received registration request");

            RegisterResponseDto registerResponse = await _authService.RegisterAgentAsync(registerRequestDto);

            APIResponses<RegisterResponseDto> apiResponse = new APIResponses<RegisterResponseDto>
            {
                Success = true,
                Message = "Agent registered successfully",
                Data = registerResponse,
                Errors = null
            };

            _logger.LogInformation($"Register: Agent {registerResponse.Email} registered successfully");
            return Created("", apiResponse);
        }


        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<PaginatedResponseDto<UserResponseDto>>>> GetAllUsers([FromQuery] PaginationRequestDto paginationRequest)
        {
            _logger.LogInformation("GetAllUsers: Received request");

            PaginatedResponseDto<UserResponseDto> users = await _authService.GetAllUsersAsync(paginationRequest);

            APIResponses<PaginatedResponseDto<UserResponseDto>> apiResponse = new APIResponses<PaginatedResponseDto<UserResponseDto>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users,
                Errors = null
            };

            _logger.LogInformation($"GetAllUsers: Returned {users.TotalCount} users");
            return Ok(apiResponse);
        }


        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<CurrentUserResponseDto>>> GetCurrentUser()
        {
            _logger.LogInformation("GetCurrentUser: Received request");

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            CurrentUserResponseDto currentUser = await _authService.GetCurrentUserAsync(userId);

            APIResponses<CurrentUserResponseDto> apiResponse = new APIResponses<CurrentUserResponseDto>
            {
                Success = true,
                Message = "Current user retrieved successfully",
                Data = currentUser,
                Errors = null
            };

            _logger.LogInformation($"GetCurrentUser: Returned user {currentUser.Email}");
            return Ok(apiResponse);
        }

        [HttpPost("create-agent")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<CreateAgentResponseDto>>> CreateAgent([FromBody] CreateAgentRequestDto requestDto)
        {
            _logger.LogInformation("CreateAgent: Received request");

            string photographyCompanyId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            CreateAgentResponseDto responseDto = await _authService.CreateAgentAsync(requestDto, photographyCompanyId);

            APIResponses<CreateAgentResponseDto> apiResponse = new APIResponses<CreateAgentResponseDto>
            {
                Success = true,
                Message = "Agent created successfully",
                Data = responseDto,
                Errors = null
            };

            _logger.LogInformation($"CreateAgent: Agent {responseDto.Email} created successfully");
            return Created("", apiResponse);
        }
    }
    
}
