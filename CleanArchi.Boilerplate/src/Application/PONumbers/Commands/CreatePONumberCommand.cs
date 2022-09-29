using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Domain.Entities;
using MediatR;

namespace CleanArchi.Boilerplate.Application.PONumbers.Commands;

public class CreatePONumberCommand : IRequest<int>
{
    public string PONumber { get; set; }
    public int ShipToNumber { get; set; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreatePONumberCommand, int>
{

    public async Task<int> Handle(CreatePONumberCommand request, CancellationToken cancellationToken)
    {
        var entity = new PONumberTest();

        entity.PONumber = request.PONumber;
        entity.ShipToNumber = request.ShipToNumber;

        //_context.TodoLists.Add(entity);

        //await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
