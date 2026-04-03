using System;
using Remp.Remp.Models;
using Remp.Remp.Models.DTOs;
using AutoMapper;

namespace Remp.Remp.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateListingCaseRequestDto, ListingCase>();
        CreateMap<ListingCase, ListingCaseResponseDto>();

        CreateMap<UpdateListingCaseRequestDto, ListingCase>();
        CreateMap<ListingCase, UpdateListingCaseRequestDto>();
    }

}
