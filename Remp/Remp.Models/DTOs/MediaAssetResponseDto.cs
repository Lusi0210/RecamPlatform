using System;
using Remp.Remp.Models.Enum;

namespace Remp.Remp.Models.DTOs;

public class MediaAssetResponseDto
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsSelect { get; set; }
    public bool IsHero { get; set; }
    public int ListingCaseId { get; set; }
}
