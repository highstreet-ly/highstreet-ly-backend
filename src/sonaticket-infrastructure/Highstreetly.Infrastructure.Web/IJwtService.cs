using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Highstreetly.Infrastructure
{
    public interface IJwtService
    {
        string CreateRequestJwt(ClaimsIdentity claims, string issuer, string audience, SigningCredentials credential, bool setJwtTyp = false);
        X509Certificate2 LoadCertificate();
        Task<bool> ValidateTokenAsync(string authToken, Func<ClaimsPrincipal, bool> validate);
        string GetAccessToken();
    }
}