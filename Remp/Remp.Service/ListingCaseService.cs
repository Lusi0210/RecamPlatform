using System;
using Remp.Remp.Models;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Models.Enum;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Service;

public class ListingCaseService : IListingCaseService
{
    private IListingCaseRepository  _listingcaseDbContext;

    public ListingCaseService(IListingCaseRepository listingCaseRepository)
    {
       _listingcaseDbContext = listingCaseRepository;
    }

    public async Task<ListingCase> CreateListingCaseAsync(ListingCase listingCase)
    {
        listingCase.ListcaseStatus = ListcaseStatus.Created;
        listingCase.CreatedAt = DateTime.UtcNow;
        listingCase.IsDeleted = false;
        ListingCase newListingCase = await _listingcaseDbContext.AddListingCaseAsync(listingCase);
        return newListingCase;
    }

    public async Task<PaginatedResponseDto<ListingCaseResponseDto>> GetAllListingCasesAsync(ListingCaseFilterRequestDto filter)
    {
        var (items, totalCount) = await _listingcaseDbContext.GetAllListingCasesAsync(filter.PageNumber, filter.PageSize, filter.Status);

        int totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        List<ListingCaseResponseDto> responseDtos = items.Select(l => new ListingCaseResponseDto
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
            Street = l.Street,
            City = l.City,
            State = l.State,
            PostCode = l.PostCode,
            Longitude = l.Longitude,
            Latitude = l.Latitude,
            Price = l.Price,
            Bedrooms = l.Bedrooms,
            Bathrooms = l.Bathrooms,
            Garages = l.Garages,
            FloorArea = l.FloorArea,
            CreatedAt = l.CreatedAt,
            PropertyType = l.PropertyType,
            SaleCategory = l.SaleCategory,
            ListcaseStatus = l.ListcaseStatus,
            UserId = l.UserId
        }).ToList();

        return new PaginatedResponseDto<ListingCaseResponseDto>
        {
            Items = responseDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<ListingCase> GetListingCaseByIdAsync(int id)
    {
        ListingCase listingCase = await _listingcaseDbContext.GetListingCaseByIdAsync(id);
        return listingCase;
        
    }

    public async Task<ListingCase> UpdateListingCaseAsync(ListingCase listingCase)
    {
        ListingCase updatedListingCase = await _listingcaseDbContext.UpdateListingCaseAsync(listingCase);
        return updatedListingCase;
    }

    public async Task<bool> DeleteListingCaseAsync(int id)
    {
        bool result = await _listingcaseDbContext.DeleteListingCaseAsync(id);
        return result;
    }

    public async Task<ListingCase> UpdateListingCaseStatusAsync(int id, ListcaseStatus newStatus)
    {
        ListingCase updatedListingCase = await _listingcaseDbContext.UpdateListingCaseStatusAsync(id, newStatus);
        return updatedListingCase;
    }

    public async Task<string> PublishListingCaseAsync(int listingCaseId)
    {
        ListingCase listingCase = await _listingcaseDbContext.GetListingCaseByIdAsync(listingCaseId);

        // Generate unique shareable URL
        string uniqueToken = Guid.NewGuid().ToString("N");
        string shareableUrl = $"https://recam.com/listing/{uniqueToken}";

        listingCase.ShareableUrl = shareableUrl;
        await _listingcaseDbContext.UpdateListingCaseAsync(listingCase);

        return shareableUrl;
    }
    
    
}
