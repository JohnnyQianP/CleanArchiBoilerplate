using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchi.Boilerplate.Application.PONumbers.Commands;

internal class CreatePONumberCommandValidator : AbstractValidator<CreatePONumberCommand>
{
    public CreatePONumberCommandValidator()
    {
        RuleFor(v => v.PONumber)
            .NotEmpty().WithMessage("PONumber is required.")
            .MaximumLength(200).WithMessage("PONumber must not exceed 50 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return false;
        //return await _context.TodoLists
        //    .AllAsync(l => l.Title != title, cancellationToken);
    }
}
