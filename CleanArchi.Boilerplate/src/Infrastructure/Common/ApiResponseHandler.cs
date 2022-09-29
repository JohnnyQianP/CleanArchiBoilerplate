using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Shared;
using CleanArchi.Boilerplate.Application.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CleanArchi.Boilerplate.Infrastructure.Common;

public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUser _user;

    public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUser user) : base(options, logger, encoder, clock)
    {
        _user = user;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE401)).MessageModel));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        if (_user.MessageModel != null)
        {
            Response.StatusCode = _user.MessageModel.status;
            await Response.WriteAsync(JsonConvert.SerializeObject(_user.MessageModel));
        }
        else
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE403)).MessageModel));
        }
    }
}
