using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchi.Boilerplate.Domain.Entities;
using CleanArchi.Boilerplate.Repository.Base;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchi.Boilerplate.Application.PONumbers.Queries;

/// <summary>
/// 1.函数式提倡的东西. 对象创建后, 属性值就不可以修改了
/// 2.Tuple 解构的特性.
/// 3.修改： clone 一个新的来赋值 var newDimension = dimension with { Height = 100 };
/// </summary>
public record GetPOQuery(int id) : IRequest<PONumberDto>;

public class GetPOQueryHandler : IRequestHandler<GetPOQuery, PONumberDto>
{
    private readonly IMapper _mapper;
    private readonly IBaseRepository<PONumberTest> _repo;
    private readonly ILogger<GetPOQueryHandler> _logger;

    public GetPOQueryHandler(IMapper mapper, IBaseRepository<PONumberTest> repo, ILogger<GetPOQueryHandler> logger)
    {
        _mapper = mapper;
        _repo = repo;
        _logger = logger;
    }

    public async Task<PONumberDto> Handle(GetPOQuery request, CancellationToken cancellationToken)
    {
        var ent = await _repo.QueryById(request.id);
        _logger.LogInformation("GetPOQuery, id: {id}", request.id);
        return _mapper.Map<PONumberDto>(ent);
    }
}