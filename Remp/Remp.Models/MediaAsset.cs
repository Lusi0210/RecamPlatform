using System;
using Remp.Remp.Models.Enum;

namespace Remp.Remp.Models;

public class MediaAsset
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; }
    public DateTime UploadedAt { get; set; }
    public Boolean IsSelect { get; set; }
    public Boolean IsHero { get; set; }
    public int ListingCaseId { get; set; }
    public string UserId { get; set; }
    public Boolean IsDeleted { get; set; }
}
