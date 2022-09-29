using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Application.Common.Auth.Token;
using CleanArchi.Boilerplate.Shared.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchi.Boilerplate.Infrastructure.Auth.Token;

internal class TokenService : ITokenService
{
    private readonly SecurityJWT _jwtSettings;
    public TokenService(IOptions<SecurityJWT> jwtSettings) 
    {
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        //todo 验证用户合法且存在

        return await GenerateTokensAndUpdateUser(null);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);

        //var user = await _userManager.FindByNameAsync(userPrincipal.Identity?.Name);
        //需要对比 refreshtoken 原来的和查询到的是否一致

        return await GenerateTokensAndUpdateUser(null);  
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user)
    {
        string token = GenerateJwt(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        return new TokenResponse(token, refreshToken, refreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user));

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user) =>
       new List<Claim>
       {
            new(ClaimTypes.NameIdentifier, "tester"),
            new(ClaimTypes.Name, "zhangsan"),
            new(ClaimTypes.Role, Permissions.SpClusterApi),
       };

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception ( "Invalid Token.");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}
