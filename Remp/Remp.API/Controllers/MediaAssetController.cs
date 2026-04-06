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

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<List<MediaAssetResponseDto>>>> UploadMediaAssets(IFormFile files, [FromForm] Remp.Models.Enum.MediaType mediaType, [FromForm] int listingCaseId)
        {
            _logger.LogInformation($"UploadMediaAssets: Received request for listing case {listingCaseId}");

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            List<IFormFile> fileList = new List<IFormFile> { files };

            List<MediaAssetResponseDto> responseDtos = await _mediaAssetService.UploadMediaAssetsAsync(fileList, mediaType, listingCaseId, userId);

            APIResponses<List<MediaAssetResponseDto>> apiResponse = new APIResponses<List<MediaAssetResponseDto>>
            {
                Success = true,
                Message = "Media assets uploaded successfully",
                Data = responseDtos,
                Errors = null
            };

            _logger.LogInformation($"UploadMediaAssets: Uploaded {responseDtos.Count} files for listing case {listingCaseId}");
            return Created("", apiResponse);
        }

        [HttpGet("download/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadMediaAsset(int id)
        {
            _logger.LogInformation($"DownloadMediaAsset: Received request for media {id}");

            var (fileStream, contentType, fileName) = await _mediaAssetService.DownloadMediaAssetAsync(id);

            _logger.LogInformation($"DownloadMediaAsset: Downloading media {id}");
            return File(fileStream, contentType, fileName);
        }

        [HttpGet("listing/{listingCaseId}/download")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadAllMedia(int listingCaseId)
        {
            _logger.LogInformation($"DownloadAllMedia: Received request for listing case {listingCaseId}");

            var (zipStream, fileName) = await _mediaAssetService.DownloadAllMediaByListingCaseAsync(listingCaseId);

            _logger.LogInformation($"DownloadAllMedia: Returning ZIP for listing case {listingCaseId}");
            return File(zipStream, "application/zip", fileName);
        }

        [HttpPut("listing/{listingCaseId}/cover-image")]
        [Authorize(Roles = "Agent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<MediaAssetResponseDto>>> SetCoverImage(int listingCaseId, [FromBody] SetCoverImageRequestDto requestDto)
        {
            _logger.LogInformation($"SetCoverImage: Received request for listing case {listingCaseId}, media {requestDto.MediaId}");

            MediaAssetResponseDto responseDto = await _mediaAssetService.SetCoverImageAsync(listingCaseId, requestDto.MediaId);

            APIResponses<MediaAssetResponseDto> apiResponse = new APIResponses<MediaAssetResponseDto>
            {
                Success = true,
                Message = "Cover image set successfully",
                Data = responseDto,
                Errors = null
            };

            _logger.LogInformation($"SetCoverImage: Media {requestDto.MediaId} set as cover for listing case {listingCaseId}");
            return Ok(apiResponse);
        }

        [HttpGet("listing/{listingCaseId}/final-selection")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<List<MediaAssetResponseDto>>>> GetFinalSelection(int listingCaseId)
        {
            _logger.LogInformation($"GetFinalSelection: Received request for listing case {listingCaseId}");

            List<MediaAssetResponseDto> selectedMedia = await _mediaAssetService.GetFinalSelectionAsync(listingCaseId);

            APIResponses<List<MediaAssetResponseDto>> apiResponse = new APIResponses<List<MediaAssetResponseDto>>
            {
                Success = true,
                Message = "Final selection retrieved successfully",
                Data = selectedMedia,
                Errors = null
            };

            _logger.LogInformation($"GetFinalSelection: Returned {selectedMedia.Count} selected media for listing case {listingCaseId}");
            return Ok(apiResponse);
        }
    }
}
