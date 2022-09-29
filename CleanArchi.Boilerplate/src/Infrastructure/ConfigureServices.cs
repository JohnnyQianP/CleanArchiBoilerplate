//using CleanArchi.Boilerplate.Application.Common.Interfaces;
using CleanArchi.Boilerplate.Infrastructure.Auth;
using CleanArchi.Boilerplate.Application.AutoMapper;
using CleanArchi.Boilerplate.Infrastructure.Common;
using CleanArchi.Boilerplate.Infrastructure.Db;
using CleanArchi.Boilerplate.Infrastructure.HttpContext;
//using CleanArchi.Boilerplate.Infrastructure.Auth;
using CleanArchi.Boilerplate.Infrastructure.MiniProfiler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCacheSetup();
        //config sqlserver 
        //dbcontext
        services.AddSqlsugarSetup(configuration);

        //miniprofile
        services.AddMiniProfilerSetup(configuration);

        //CORS
        services.AddCorsSetup(configuration);

        services.AddHttpContextSetup();

        //config authorization policy & roles & permission
        services.AddAuthorizationPermission();

        if (configuration.GetValue<bool>("SetupFlag:UseIdentityserver"))
        {
            //config authenticate service: 1 identityserver
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration.GetValue<string>("IdentityServer4:AuthorizationUrl");
                    options.RequireHttpsMetadata = false;
                    options.Audience = configuration.GetValue<string>("IdentityServer4:ApiName");
                })
                .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });
        }
        else {
            //config authenticate service: 2 jwt
            services.AddJWTAuthenticationService(configuration);
        }
        return services;
    }

    private static void AddMemoryCacheSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped<ICacheService, SqlSugarMemoryCacheService>();
        services.AddSingleton<IMemoryCache>(factory =>
        {
            var value = factory.GetRequiredService<IOptions<MemoryCacheOptions>>();
            var cache = new MemoryCache(value);
            return cache;
        });
    }

    private static void AddCorsSetup(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddCors(c =>
        {
            bool allIPsEnabled = configuration.GetValue<bool>("Cors:EnableAllIPs");
            if (!allIPsEnabled)
            {
                c.AddPolicy(configuration.GetValue<string>("Cors:PolicyName"),
                    policy =>
                    {
                        policy
                        .WithOrigins(configuration.GetValue<string>("Cors:IPs").Split(','))
                        .AllowAnyHeader()//Ensures that the policy allows any header.
                        .AllowAnyMethod();
                    });
            }
            else
            {
                //允许任意跨域请求
                c.AddPolicy(configuration.GetValue<string>("Cors:PolicyName"),
                    policy =>
                    {
                        policy
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            }

        });
    }
}
