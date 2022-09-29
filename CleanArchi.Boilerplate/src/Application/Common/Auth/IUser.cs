using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Shared;

namespace CleanArchi.Boilerplate.Application.Common.Auth;

public interface IUser
{
    string Name { get; }
    int ID { get; }
    bool IsAuthenticated();
    IEnumerable<Claim> GetClaimsIdentity();
    List<string> GetClaimValueByType(string ClaimType);

    string GetToken();
    List<string> GetUserInfoFromToken(string ClaimType);

    MessageModel<string> MessageModel { get; set; }
}
