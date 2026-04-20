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
    public class AgentListingCaseController : ControllerBase
    {
        private readonly IAgentListingCaseService _agentListingCaseService;
        private readonly ILogger<AgentListingCaseController> _logger;

        public AgentListingCaseController(IAgentListingCaseService agentListingCaseService, ILogger<AgentListingCaseController> logger)
        {
            _agentListingCaseService = agentListingCaseService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<AddAgentToListingCaseRequestDto>>> AddAgentToListingCase([FromBody] AddAgentToListingCaseRequestDto requestDto)
        {
            _logger.LogInformation($"AddAgentToListingCase: Adding agent {requestDto.AgentId} to listing case {requestDto.ListingCaseId}");

            AddAgentToListingCaseRequestDto responseDto = await _agentListingCaseService.AddAgentToListingCaseAsync(requestDto.AgentId, requestDto.ListingCaseId);

            APIResponses<AddAgentToListingCaseRequestDto> apiResponse = new APIResponses<AddAgentToListingCaseRequestDto>
            {
                Success = true,
                Message = "Agent added to listing case successfully",
                Data = responseDto,
                Errors = null
            };

            _logger.LogInformation($"AddAgentToListingCase: Agent {requestDto.AgentId} added to listing case {requestDto.ListingCaseId}");
            return Created("", apiResponse);
        }
    }
}
