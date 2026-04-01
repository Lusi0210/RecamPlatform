using System;

namespace Remp.Remp.Models;

public class MediaAsset
{
    public int Id { get; set; }
    public int ListingCaseId { get; set; }
    public string Filename { get; set; }
    public string FileType { get; set; }
    public string FileUrl { get; set; }
    public bool IsHeroMedia { get; set; }
}
