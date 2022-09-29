using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchi.Boilerplate.Application.PONumbers.Queries;

public class GetPOQueryValidator : AbstractValidator<GetPOQuery>
{
    public GetPOQueryValidator()
    {
        RuleFor(v => v.id)
            .NotEmpty().WithMessage("id is required.")
            .GreaterThan(0).WithMessage("must greater than 0");
    }

}
