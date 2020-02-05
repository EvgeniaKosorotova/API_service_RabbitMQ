using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QueueMessageSender.Logic
{
    public class AuthenticationJWT
    {
        private static AuthenticationJWT _instance = null;
        private IConfiguration _configuration;

        public static AuthenticationJWT Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AuthenticationJWT();
                return _instance;
            }
        }
        public IConfiguration Configuration
        {
            set => _configuration = value;
        }
        private AuthenticationJWT()
        {
        }

        public Dictionary<string, string> CreateTokens(string username) 
        {
            var accessToken = CreateAccessToken(username);
            var refreshToken = CreateRefreshToken();
            var infoTokens = new Dictionary<string, string>()
            {
                {"accessToken", accessToken},
                {"refreshToken", refreshToken}
            };
            return infoTokens;
        }

        private string CreateAccessToken(string username) 
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };
            var jwtSecurityKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Settings:JWT:SecurityKey"));
            var key = new SymmetricSecurityKey(jwtSecurityKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Settings:JWT:AccessToken:ExpiryInMinutes"));

            var accessToken = new JwtSecurityToken(
                _configuration.GetValue<string>("Settings:JWT:Issuer"),
                _configuration.GetValue<string>("Settings:JWT:Audience"),
                claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: creds
                );
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
            return accessTokenString;
        }

        private string CreateRefreshToken()
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
