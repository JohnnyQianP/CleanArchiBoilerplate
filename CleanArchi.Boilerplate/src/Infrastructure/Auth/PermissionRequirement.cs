using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 用户权限集合，一个订单包含了很多详情，
    /// 同理，一个网站的认证发行中，也有很多权限详情(这里是Role和URL的关系)
    /// </summary>
    //public List<string> Permissions { get; set; }
    public string[] Permissions { get; set; }

    //todo 集成thirdparty call permission

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="permissions">权限集合</param>
    public PermissionRequirement(string[] permissions)
    {
        Permissions = permissions;
    }
}