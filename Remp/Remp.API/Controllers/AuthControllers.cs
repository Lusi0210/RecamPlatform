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
    }
}
