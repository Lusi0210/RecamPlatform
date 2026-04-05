using System;
using Remp.Remp.Models.Entities;

namespace Remp.Remp.Models.Interfaces.Repositories;

public interface ICaseContactRepository
{
    Task<List<CaseContact>> GetCaseContactsByListingCaseIdAsync(int listingCaseId);
}
