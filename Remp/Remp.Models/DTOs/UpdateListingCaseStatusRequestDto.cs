using System;

namespace Remp.Remp.Models.DTOs;

public class UpdateListingCaseStatusRequestDto
{
    public Enum.ListcaseStatus ListcaseStatus { get; set; }
}
