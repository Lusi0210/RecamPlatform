using System;
using Remp.Remp.Models.Enum;

namespace Remp.Remp.Models.DTOs;

public class ListingCaseFilterRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ListcaseStatus? Status { get; set; }
}
