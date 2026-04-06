using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.API.Validators;

public class AddAgentToCompanyRequestDtoValidator : AbstractValidator<AddAgentToCompanyRequestDto>
{
    public AddAgentToCompanyRequestDtoValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty();
    }
}
