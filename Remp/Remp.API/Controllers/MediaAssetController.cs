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
    public class MediaAssetController : ControllerBase
    {
        private readonly IMediaAssetService _mediaAssetService;
        private readonly ILogger<MediaAssetController> _logger;

        public MediaAssetController(IMediaAssetService mediaAssetService, ILogger<MediaAssetController> logger)
        {
            _mediaAssetService = mediaAssetService;
            _logger = logger;
        }

        [HttpGet("listing/{listingCaseId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<List<GroupedMediaResponseDto>>>> GetMediaByListingCaseId(int listingCaseId)
        {
            _logger.LogInformation($"GetMediaByListingCaseId: Received request for listing case {listingCaseId}");

            List<GroupedMediaResponseDto> groupedMedia = await _mediaAssetService.GetMediaByListingCaseIdAsync(listingCaseId);

            APIResponses<List<GroupedMediaResponseDto>> apiResponse = new APIResponses<List<GroupedMediaResponseDto>>
            {
                Success = true,
                Message = "Media assets retrieved successfully",
                Data = groupedMedia,
                Errors = null
            };

            _logger.LogInformation($"GetMediaByListingCaseId: Returned media for listing case {listingCaseId}");
            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            _logger.LogInformation($"DeleteMedia: Received request to delete media {id}");

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            bool result = await _mediaAssetService.DeleteMediaAsync(id, userId);
            if (result)
            {
                _logger.LogInformation($"DeleteMedia: Successfully deleted media {id}");
                return NoContent();
            }

            throw new Exception("Failed to delete media asset.");
        }
    }
}
