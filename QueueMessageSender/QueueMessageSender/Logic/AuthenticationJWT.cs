using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QueueMessageSender.Logic
{
    public class AuthenticationJWT
    {
        private readonly IConfiguration _configuration;

        public AuthenticationJWT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(string username) 
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };
            var jwtSecurityKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Settings:JWT:SecurityKey"));
            var key = new SymmetricSecurityKey(jwtSecurityKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.Add(TimeSpan.ParseExact(_configuration.GetSection("Settings:JWT:AccessToken:Expiry").Value, "c", null));

            var accessToken = new JwtSecurityToken(
                _configuration.GetValue<string>("Settings:JWT:Issuer"),
                _configuration.GetValue<string>("Settings:JWT:Audience"),
                claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
                );
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
            return accessTokenString;
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[16];
            using (var randomGenerator = RandomNumberGenerator.Create()) 
            {
                randomGenerator.GetBytes(randomNumber);
                var refreshTokenString = Convert.ToBase64String(randomNumber);
                return refreshTokenString;
            }
        }
    }
}
