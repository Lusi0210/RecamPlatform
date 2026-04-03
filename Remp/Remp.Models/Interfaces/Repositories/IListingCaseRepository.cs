using System;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IListingCaseRepository
{
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
    Task<List<ListingCase>> GetAllListingCasesAsync();
    Task<ListingCase> GetListingCaseByIdAsync(int id);
    Task<ListingCase> UpdateListingCaseAsync(ListingCase listingCase);
    Task<bool> DeleteListingCaseAsync(int id);
    Task<ListingCase> UpdateListingCaseStatusAsync(int id, Enum.ListcaseStatus newStatus);
}
