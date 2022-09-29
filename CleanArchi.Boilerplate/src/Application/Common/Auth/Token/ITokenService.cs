using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Application.Common.Interfaces;

namespace CleanArchi.Boilerplate.Application.Common.Auth.Token;

public interface ITokenService: ITransientService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken);

    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}
