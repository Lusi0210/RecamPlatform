using System;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.Models.Interfaces.Services;

public interface ICaseContactService
{
    Task<List<CaseContactResponseDto>> GetCaseContactsByListingCaseIdAsync(int listingCaseId);
}   
