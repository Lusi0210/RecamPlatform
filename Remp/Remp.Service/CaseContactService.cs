using System;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Repositories;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class CaseContactService : ICaseContactService
{
    private readonly ICaseContactRepository _caseContactRepository;

    public CaseContactService(ICaseContactRepository caseContactRepository)
    {
        _caseContactRepository = caseContactRepository;
    }

    public async Task<List<CaseContactResponseDto>> GetCaseContactsByListingCaseIdAsync(int listingCaseId)
    {
        List<CaseContact> caseContacts = await _caseContactRepository.GetCaseContactsByListingCaseIdAsync(listingCaseId);

        List<CaseContactResponseDto> responseDtos = caseContacts.Select(c => new CaseContactResponseDto
        {
            ContactId = c.ContactId,
            FirstName = c.FirstName,
            LastName = c.LastName,
            CompanyName = c.CompanyName,
            ProfileUrl = c.ProfileUrl,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            ListingCaseId = c.ListingCaseId
        }).ToList();

        return responseDtos;
    }
}
