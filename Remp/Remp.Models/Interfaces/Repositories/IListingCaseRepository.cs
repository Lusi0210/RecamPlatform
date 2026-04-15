using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface IListingCaseRepository
{
    Task<ListingCase> AddListingCaseAsync(ListingCase listingCase);
    Task<(List<ListingCase> Items, int TotalCount)> GetAllListingCasesAsync(int pageNumber, int pageSize, Enum.ListcaseStatus? status);
    Task<ListingCase> GetListingCaseByIdAsync(int id);
    Task<ListingCase> UpdateListingCaseAsync(ListingCase listingCase);
    Task<bool> DeleteListingCaseAsync(int id);
    Task<ListingCase> UpdateListingCaseStatusAsync(int id, Enum.ListcaseStatus newStatus);
}
