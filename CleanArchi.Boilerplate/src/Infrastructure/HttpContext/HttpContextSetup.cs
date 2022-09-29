using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Application.Common.Auth;
using CleanArchi.Boilerplate.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchi.Boilerplate.Infrastructure.HttpContext;

/// <summary>
/// HttpContext 相关服务
/// </summary>
public static class HttpContextSetup
{
    public static void AddHttpContextSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUser, ApplicationUser>();
    }
}
