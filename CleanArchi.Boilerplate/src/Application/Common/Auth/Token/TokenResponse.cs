using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Application.Common.Auth.Token;

public record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime);
