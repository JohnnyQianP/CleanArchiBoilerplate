using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

/// <summary>
/// 继承AuthorizeAttribute，auto-call IAuthorizationService.AuthorizeAsync()
/// </summary>
public class NeedPermissionAttribute : AuthorizeAttribute
{
    public NeedPermissionAttribute(params string[] actionResource) =>
        Policy = string.Join(",", actionResource);// actionResource $"Permissions.{actionResource}";
}

/// <summary>
/// 为第三方调用
/// </summary>
public class ThirdPartyAttribute : AuthorizeAttribute
{
    public ThirdPartyAttribute(params string[] actionResource) =>
        Policy = string.Join(",", actionResource);// actionResource $"Permissions.{actionResource}";
}
