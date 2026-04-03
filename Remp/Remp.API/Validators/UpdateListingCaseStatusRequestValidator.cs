using System;
using FluentValidation;
using Remp.Remp.Models.DTOs;
namespace Remp.Remp.API.Validators;

public class UpdateListingCaseStatusRequestValidator : AbstractValidator<UpdateListingCaseStatusRequestDto>
{
    public UpdateListingCaseStatusRequestValidator()
    {
        RuleFor(x => x.ListcaseStatus)
            .IsInEnum()
            .WithMessage("Invalid listing case status. Allowed values are: Created, Pending, Delivered.");
    }
}
