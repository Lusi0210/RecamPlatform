using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;

namespace Remp.Remp.API.Validators;

public class SelectMediaRequestDtoValidator : AbstractValidator<SelectMediaRequestDto>
{
    public SelectMediaRequestDtoValidator()
    {
        RuleFor(x => x.MediaIds)
            .NotEmpty()
            .Must(ids => ids.Count <= 10)
            .WithMessage("Maximum 10 images can be selected.");
    }
}
