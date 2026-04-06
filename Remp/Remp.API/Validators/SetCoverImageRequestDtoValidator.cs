using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.API.Validators;

public class SetCoverImageRequestDtoValidator : AbstractValidator<SetCoverImageRequestDto>
{
    public SetCoverImageRequestDtoValidator()
    {
        RuleFor(x => x.MediaId)
            .GreaterThan(0);
    }
}
