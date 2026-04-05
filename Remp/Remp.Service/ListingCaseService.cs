using System;
using Remp.Remp.Models;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;
using Remp.Remp.Models.Enum;
using Remp.Remp.Models.Entities;

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

    public async Task<List<ListingCase>> GetAllListingCasesAsync()
    {
        List<ListingCase> listingCases = await _listingcaseDbContext.GetAllListingCasesAsync();
        return listingCases;
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
    
    
}
