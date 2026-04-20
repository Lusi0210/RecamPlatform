using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.API.Validators;

public class AddAgentToListingCaseRequestDtoValidator : AbstractValidator<AddAgentToListingCaseRequestDto>
{   
    public AddAgentToListingCaseRequestDtoValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty();

        RuleFor(x => x.ListingCaseId)
            .GreaterThan(0);
    }
}
