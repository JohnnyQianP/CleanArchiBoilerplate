using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Shared;
using CleanArchi.Boilerplate.Infrastructure.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using CleanArchi.Boilerplate.Shared.Configuration;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

internal static class JWTConfigService
{
    public static void AddJWTAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var symmetricKeyAsBase64 = configuration.GetValue<string>("SecurityJWT:Secret");
        var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
        var signingKey = new SymmetricSecurityKey(keyByteArray);
        var Issuer = configuration.GetValue<string>("SecurityJWT:Issuer");
        var Audience = configuration.GetValue<string>("SecurityJWT:Audience");

        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // 令牌验证参数
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = Issuer,//发行人
            ValidateAudience = true,
            ValidAudience = Audience,//订阅人
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RequireExpirationTime = true,
        };

        // 开启Bearer认证
        services.AddAuthentication(o =>
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = nameof(ApiResponseHandler);
            o.DefaultForbidScheme = nameof(ApiResponseHandler);
        })
         // 添加JwtBearer服务
         .AddJwtBearer(o =>
         {
             o.TokenValidationParameters = tokenValidationParameters;
             o.Events = new JwtBearerEvents
             {
                 OnChallenge = context =>
                 {
                     context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                     return Task.CompletedTask;
                 },
                 OnAuthenticationFailed = context =>
                 {
                     var jwtHandler = new JwtSecurityTokenHandler();
                     var token = context.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "");

                     if (token.IsNotEmptyOrNull() && jwtHandler.CanReadToken(token))
                     {
                         var jwtToken = jwtHandler.ReadJwtToken(token);

                         if (jwtToken.Issuer != Issuer)
                         {
                             context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
                         }

                         if (jwtToken.Audiences.FirstOrDefault() != Audience)
                         {
                             context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
                         }
                     }


                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                     {
                         context.Response.Headers.Add("Token-Expired", "true");
                     }
                     return Task.CompletedTask;
                 }
             };
         })
         .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });

        services.Configure<SecurityJWT>(configuration.GetSection(nameof(SecurityJWT)));
    }
}
