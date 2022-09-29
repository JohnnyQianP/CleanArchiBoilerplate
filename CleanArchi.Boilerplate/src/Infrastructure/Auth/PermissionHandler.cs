using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Application.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// 验证方案提供对象
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; set; }

    //private readonly IRoleModulePermissionServices _roleModulePermissionServices;
    private readonly IHttpContextAccessor _accessor;
    //private readonly ISysUserInfoServices _userServices;
    private readonly IUser _user;

    /// <summary>
    /// 构造函数注入
    /// </summary>
    /// <param name="schemes"></param>
    /// <param name="roleModulePermissionServices"></param>
    /// <param name="accessor"></param>
    /// <param name="userServices"></param>
    /// <param name="user"></param>
    public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor, IUser user)
    {
        _accessor = accessor;
        //_userServices = userServices;
        _user = user;
        Schemes = schemes;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        //permission handle
        //requirement.Permissions

        //获取roles permission

        //获取登录状态

        //赋值context user,检查role

        //_accessor.HttpContext.User = ;
        var user = _accessor.HttpContext.User.Claims;
        var user1 = context.User.Claims;
        var need = requirement.Permissions;
            //context.User?.GetUserId() is { } userId &&  await _userService.HasPermissionAsync(userId, requirement.Permissions)
        if (user.Any(a=>a.Type== ClaimTypes.Role && need.Contains(a.Value)))
        {
            context.Succeed(requirement);
        }
        return;
    }
}
