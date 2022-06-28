using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Highstreetly.Infrastructure
{
    public class JwtService : IJwtService
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(ILogger logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public JwtService()
        {

        }

        public string CreateRequestJwt(ClaimsIdentity claims, string issuer, string audience, SigningCredentials credential, bool setJwtTyp = false)
        {

            var handler = new JwtSecurityTokenHandler();
            handler.OutboundClaimTypeMap.Clear();

            var token = handler.CreateJwtSecurityToken(
                issuer: issuer,
                audience: audience,
                signingCredentials: credential,
                subject: claims
                //TODO: need to add expires
            );

            if (setJwtTyp)
            {
                token.Header["typ"] = JwtClaimTypes.JwtTypes.AuthorizationRequest;
            }

            return handler.WriteToken(token);
        }

        public X509Certificate2 LoadCertificate()
        {
            var key = $"{_configuration.GetSection("Ssl")["Path"]}tls.crt";

            if (_cache?.Get(key) is X509Certificate2 cert)
            {
                return cert;
            }
            else
            {
                return LoadCertificateInternal(key);
            }
        }

        private X509Certificate2 LoadCertificateInternal(string key)
        {
            bool exceptionally = false;
            while (!exceptionally)
            {
                try
                {
                    // kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.crt"]' | tr -d '"' | base64 -d  > ssl/server.crt
                    // kubectl get secret crt-ids.$DOMAIN -n $NAMESPACE -o json | jq '.data["tls.key"]' | tr -d '"' | base64 -d  > ssl/server.key

                    var sslPath = _configuration.GetSection("Ssl")["Path"];

                    var cert = new X509Certificate2($"{sslPath}tls.crt");

                    var rsa = RSA.Create();
                    var content = File
                        .ReadAllText($"{sslPath}tls.key")
                        .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                        .Replace("-----END RSA PRIVATE KEY-----", "")
                        .Replace("\n", "");

                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(content), out int bytesRead);

                    cert = cert.CopyWithPrivateKey(rsa);

                    _cache?.Set(key, cert, DateTime.UtcNow.AddHours(1));

                    return cert;
                }
                catch (Exception)
                {
                    exceptionally = true;
                    Thread.Sleep(10000);
                }
            }

            return null;
        }

        public async Task<bool> ValidateTokenAsync(string authToken, Func<ClaimsPrincipal, bool> validate)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.InboundClaimTypeMap.Clear();

            var validationParameters = await GetValidationParametersAsync();

            try
            {
                var result = tokenHandler.ValidateToken(authToken, validationParameters, out _);

                return validate(result);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetAccessToken()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ArgumentException("_httpContextAccessor.HttpContext");
            }

            if (_httpContextAccessor.HttpContext.Request.Headers["Authorization"].Count < 1)
            {
                return default;
            }

            return _httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].Remove(0, "Bearer ".Length);
        }

        private async Task<TokenValidationParameters> GetValidationParametersAsync()
        {
            var identityUrl = _configuration.GetIdsUrl();
            var audience = _configuration.GetSection("IdentityServer")["Audience"];

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(identityUrl);

            var keys = new List<SecurityKey>();
            foreach (var webKey in disco.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            return new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidateAudience = false,
                RequireSignedTokens = true,
                ValidIssuer = disco.Issuer,
                ValidAudience = audience,
                IssuerSigningKeys = keys
            };
        }
    }
}