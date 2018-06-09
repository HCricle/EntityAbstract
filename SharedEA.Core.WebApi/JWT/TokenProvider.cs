using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SharedEA.Core.WebApi.JWT
{
    public class TokenProvider
    {
        private readonly TokenProviderOptions _options;
        public TokenProvider(TokenProviderOptions options)
        {
            _options = options;
        }
        public async Task<TokenEntity> GenerateToken(string userName,string password)
        {
            var identity = await GetIdentity(userName);
            if (identity == null)
                return null;
            var now = DateTime.Now;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,userName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,ToUnixEpochDate(now).ToString(),ClaimValueTypes.Integer64),
                identity.FindFirst("forclaim")
            };
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: _options.Expiration,
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenEntity()
            {
                AccessToken=encodedJwt,
                ExpiresIn=(int)_options.ValidFor.TotalSeconds
            };   
        }
        private Task<ClaimsIdentity> GetIdentity(string username)
        {
            return Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(username,"Token"),
                new Claim[] 
                {
                    new Claim( ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Authentication,"logined"),
                    new Claim("forclaim","logined")
                }
                ));
        }
        public static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}
