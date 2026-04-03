using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Repository;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IListingCaseService
{
    Task<ListingCase> CreateListingCaseAsync(ListingCase listingCaseCreateDto);
    Task<List<ListingCase>> GetAllListingCasesAsync();
    Task<ListingCase> GetListingCaseByIdAsync(int id);
    Task<ListingCase> UpdateListingCaseAsync(ListingCase listingCase);
    Task<bool> DeleteListingCaseAsync(int id);
    Task<ListingCase> UpdateListingCaseStatusAsync(int id, Enum.ListcaseStatus newStatus);
}
