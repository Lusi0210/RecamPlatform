using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.API.Validators;

public class CreateAgentRequestDtoValidator : AbstractValidator<CreateAgentRequestDto>
{
    public CreateAgentRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.AgentFirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AgentLastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
