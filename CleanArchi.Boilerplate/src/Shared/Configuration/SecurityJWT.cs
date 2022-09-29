using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Shared.Configuration;

public class SecurityJWT
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }

    public int JobExpirationInDays { get; set; }
}
