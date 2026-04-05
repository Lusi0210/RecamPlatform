using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Remp.Common.Responses;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseContactController : ControllerBase
    {
        private readonly ICaseContactService _caseContactService;
        private readonly ILogger<CaseContactController> _logger;

        public CaseContactController(ICaseContactService caseContactService, ILogger<CaseContactController> logger)
        {
            _caseContactService = caseContactService;
            _logger = logger;
        }

        [HttpGet("listing/{listingCaseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<List<CaseContactResponseDto>>>> GetCaseContactsByListingCaseId(int listingCaseId)
        {
            _logger.LogInformation($"GetCaseContacts: Received request for listing case {listingCaseId}");

            List<CaseContactResponseDto> caseContacts = await _caseContactService.GetCaseContactsByListingCaseIdAsync(listingCaseId);

            APIResponses<List<CaseContactResponseDto>> apiResponse = new APIResponses<List<CaseContactResponseDto>>
            {
                Success = true,
                Message = "Case contacts retrieved successfully",
                Data = caseContacts,
                Errors = null
            };

            _logger.LogInformation($"GetCaseContacts: Returned {caseContacts.Count} contacts for listing case {listingCaseId}");
            return Ok(apiResponse);
        }
    }
}
