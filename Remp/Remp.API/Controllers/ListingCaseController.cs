using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remp.Remp.Models;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Common.Responses;

namespace Remp.Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingCaseController : ControllerBase
    {
        private IListingCaseService _listingCaseService;  
        private IMapper _mapper; 
        private ILogger<ListingCaseController> _logger;

        public ListingCaseController(IListingCaseService listingCaseService, IMapper mapper, ILogger<ListingCaseController> logger)
        {
            _listingCaseService = listingCaseService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<ListingCaseResponseDto>>> CreateListingCase([FromBody] CreateListingCaseRequestDto listingCaseCreateDto)
        {
            _logger.LogInformation("Create ListingCase: Received request");
            ListingCase listingCase = _mapper.Map<ListingCase>(listingCaseCreateDto);
            ListingCase newListingCase = await _listingCaseService.CreateListingCaseAsync(listingCase);
            ListingCaseResponseDto responseDto = _mapper.Map<ListingCaseResponseDto>(newListingCase);
            APIResponses<ListingCaseResponseDto> apiResponse = new APIResponses<ListingCaseResponseDto>
            {
                Success = true,
                Message = "Listing case created successfully",
                Data = responseDto,
                Errors = null
            };
            _logger.LogInformation($"Create ListingCase: ListingCase created with id {newListingCase.Id}");
            return Created("GetListingCaseById", apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<List<ListingCaseResponseDto>>>> GetAllListingCases()
        {
            _logger.LogInformation("Get All ListingCases: Received request");
            List<ListingCase> listingCases = await _listingCaseService.GetAllListingCasesAsync();
            List<ListingCaseResponseDto> responseDtos = _mapper.Map<List<ListingCaseResponseDto>>(listingCases);
            APIResponses<List<ListingCaseResponseDto>> apiResponse = new APIResponses<List<ListingCaseResponseDto>>
            {
                Success = true,
                Message = "Listing cases retrieved successfully",
                Data = responseDtos,
                Errors = null
            };
            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<ListingCaseResponseDto>>> GetListingCaseById(int id)
        {
            _logger.LogInformation($"Get ListingCase By Id: Received request for id {id}");
            
            ListingCase listingCase = await _listingCaseService.GetListingCaseByIdAsync(id);
            if (listingCase == null)
            {
                _logger.LogWarning($"Get ListingCase By Id: ListingCase with id {id} not found");
                return NotFound();
            }
            ListingCaseResponseDto responseDto = _mapper.Map<ListingCaseResponseDto>(listingCase);
            APIResponses<ListingCaseResponseDto> apiResponse = new APIResponses<ListingCaseResponseDto>
            {
                Success = true,
                Message = "Listing case retrieved successfully",
                Data = responseDto,
                Errors = null
            };
             _logger.LogInformation($"Get ListingCase By Id: Returning listing case with id {id}");
            return Ok(apiResponse);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponses<ListingCaseResponseDto>>> UpdateListingCase(int id, [FromBody] UpdateListingCaseRequestDto listingCaseUpdateDto)
        {
            _logger.LogInformation($"Update ListingCase: Received request to update listing case with id {id}");
            
            ListingCase existingListingCase = await _listingCaseService.GetListingCaseByIdAsync(id);
            if (existingListingCase == null)
            {
                _logger.LogWarning($"Update ListingCase: Listing case with id {id} not found");
                return NotFound();
            }

            _mapper.Map(listingCaseUpdateDto, existingListingCase);
            ListingCase updatedListingCase = await _listingCaseService.UpdateListingCaseAsync(existingListingCase);
            ListingCaseResponseDto responseDto = _mapper.Map<ListingCaseResponseDto>(updatedListingCase);
            APIResponses<ListingCaseResponseDto> apiResponse = new APIResponses<ListingCaseResponseDto>
            {
                Success = true,
                Message = "Listing case updated successfully",
                Data = responseDto,
                Errors = null
            };
            _logger.LogInformation($"Update ListingCase: Successfully updated listing case with id {id}");
            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteListingCase(int id)
        {
            _logger.LogInformation($"Delete ListingCase: Received request to delete listing case with id {id}");
            
            ListingCase existingListingCase = await _listingCaseService.GetListingCaseByIdAsync(id);
            if (existingListingCase == null)
            {
                _logger.LogWarning($"Delete ListingCase: Listing case with id {id} not found");
                return NotFound();
            }

            bool result = await _listingCaseService.DeleteListingCaseAsync(id);
            if (result)
            {
                _logger.LogInformation($"Delete ListingCase: Successfully deleted listing case with id {id}");
                return NoContent();
            }
            else
            {
                _logger.LogError($"Delete ListingCase: Failed to delete listing case with id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete listing case");
            }
        
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponses<ListingCaseResponseDto>>> UpdateListingCaseStatus(int id, [FromBody] UpdateListingCaseStatusRequestDto statusUpdateDto)
        {
            _logger.LogInformation($"Update ListingCase Status: Received request to update status of listing case with id {id}");
            
            ListingCase existingListingCase = await _listingCaseService.GetListingCaseByIdAsync(id);
            if (existingListingCase == null)
            {
                _logger.LogWarning($"Update ListingCase Status: Listing case with id {id} not found");
                return NotFound();
            }

            ListingCase updatedListingCase = await _listingCaseService.UpdateListingCaseStatusAsync(id, statusUpdateDto.ListcaseStatus);
            ListingCaseResponseDto responseDto = _mapper.Map<ListingCaseResponseDto>(updatedListingCase);
            APIResponses<ListingCaseResponseDto> apiResponses = new APIResponses<ListingCaseResponseDto>
            {
                Success = true,
                Message = "Listing case status updated successfully",
                Data = responseDto,
                Errors = null
            };
            _logger.LogInformation($"Update ListingCase Status: Successfully updated status of listing case with id {id}");
            return Ok(apiResponses);
        }

    }

}
