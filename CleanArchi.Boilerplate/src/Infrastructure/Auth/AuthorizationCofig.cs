using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

internal static class AuthorizationCofig
{
    public static void AddAuthorizationPermission(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        #region 简单方式Policy、Role不考虑
        // 1、这个很简单，其他什么都不用做， 只需要在API层的controller上边，增加特性即可
        // [Authorize(Roles = "Admin,System")]
        // 2、这个和上边的异曲同工，好处就是不用在controller中，写多个 roles 。
        // [Authorize(Policy = "Admin")]
        // services.AddAuthorization(options => { options.AddPolicy("Client", policy => policy.RequireRole("Client").Build()); );
        #endregion
        services.AddAuthorization(options => { options.AddPolicy(Permissions.JobApi, policy => { policy.RequireClaim(ClaimTypes.Role, Permissions.JobApi); }); });

        #region 深入策略 IAuthorizationHandler
        // 这里冗余写了一次
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        // 注入权限处理器
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        //notice: 深入策略。取代AddAuthorization =>addpolicy
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        #endregion
    }


}
