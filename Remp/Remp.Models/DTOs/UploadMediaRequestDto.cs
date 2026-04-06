using System;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Remp.Remp.Models.DTOs;

public class UploadMediaRequestDto
{
    public MediaType MediaType { get; set; }
    public int ListingCaseId { get; set; }
}
