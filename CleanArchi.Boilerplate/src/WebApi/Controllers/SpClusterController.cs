using CleanArchi.Boilerplate.Application.PONumbers.Queries;
using CleanArchi.Boilerplate.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Boilerplate.WebApi.Controllers;


public class SpClusterController : ApiControllerBase
{
    [HttpGet("search")]
    [NeedPermission(Permissions.SpClusterApi)]
    public async Task<PONumberDto> Get(int id)
    {
        var pon = await Mediator.Send(new GetPOQuery(id));
        return pon;
    }
}
