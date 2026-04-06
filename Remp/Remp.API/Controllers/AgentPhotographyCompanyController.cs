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
    public class AgentPhotographyCompanyController : ControllerBase
    {
        private readonly IAgentPhotographyCompanyService _agentPhotographyCompanyService;
        private readonly ILogger<AgentPhotographyCompanyController> _logger;

        public AgentPhotographyCompanyController(IAgentPhotographyCompanyService agentPhotographyCompanyService, ILogger<AgentPhotographyCompanyController> logger)
        {
            _agentPhotographyCompanyService = agentPhotographyCompanyService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<AgentPhotographyCompanyResponseDto>>> AddAgentToCompany([FromBody] AddAgentToCompanyRequestDto requestDto)
        {
            _logger.LogInformation($"AddAgentToCompany: Received request to add agent {requestDto.AgentId}");

            string photographyCompanyId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            AgentPhotographyCompanyResponseDto responseDto = await _agentPhotographyCompanyService.AddAgentToCompanyAsync(requestDto.AgentId, photographyCompanyId);

            APIResponses<AgentPhotographyCompanyResponseDto> apiResponse = new APIResponses<AgentPhotographyCompanyResponseDto>
            {
                Success = true,
                Message = "Agent added to photography company successfully",
                Data = responseDto,
                Errors = null
            };

            _logger.LogInformation($"AddAgentToCompany: Agent {requestDto.AgentId} added successfully");
            return Created("", apiResponse);
        }
    }
}
