using System;
using Remp.Remp.Models.Enum;

namespace Remp.Remp.Models.DTOs;

public class GroupedMediaResponseDto
{
    public MediaType MediaType { get; set; }
    public List<MediaAssetResponseDto> MediaAssets { get; set; }
}
