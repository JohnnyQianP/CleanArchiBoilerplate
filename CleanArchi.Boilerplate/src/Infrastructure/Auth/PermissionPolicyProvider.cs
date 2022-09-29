using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CleanArchi.Boilerplate.Infrastructure.Auth;

internal class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider, IAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => base.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {

        if (policyName.StartsWith("Permissions", StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName.Split(',')));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }

        return base.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => base.GetFallbackPolicyAsync();// Task.FromResult<AuthorizationPolicy?>(null);
}