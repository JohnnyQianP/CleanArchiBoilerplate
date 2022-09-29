using CleanArchi.Boilerplate.Application.Common.Auth.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Boilerplate.WebApi.Controllers;
public class TokensController : ApiControllerBase
{
    private readonly ITokenService _tokenService;
    public TokensController(ITokenService tokenService) 
    {
        _tokenService = tokenService;
    }
    
    [HttpPost("gettoken")]
    [AllowAnonymous]
    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken) 
    {
        return await _tokenService.GetTokenAsync(request, cancellationToken);
    }
}
