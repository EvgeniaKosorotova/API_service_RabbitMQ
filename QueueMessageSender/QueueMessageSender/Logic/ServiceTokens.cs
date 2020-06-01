using Microsoft.Extensions.Configuration;
using QueueMessageSender.Models;
using System;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public class ServiceTokens
    {
        private readonly AuthenticationJWT _authenticationJWT;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;

        public ServiceTokens(AuthenticationJWT authenticationJWT, 
                                IConfiguration configuration,
                                ITokenManager tokenManager)
        {
            _authenticationJWT = authenticationJWT;
            _configuration = configuration;
            _tokenManager = tokenManager;
        }
        public async Task<AuthenticationResultModel> CreateTokensAsync(UserModel user)
        {
            var accessToken = _authenticationJWT.CreateAccessToken(user.Username);
            var refreshToken = _authenticationJWT.CreateRefreshToken();

            await _tokenManager.AddTokenAsync(user, refreshToken);

            return new AuthenticationResultModel
            {
                AccessToken = accessToken,
                LifeTime = _configuration.GetValue<TimeSpan>("Settings:JWT:AccessToken:Expiry"),
                RefreshToken = refreshToken,
                Role = user.Role.Role
            };
        }
    }
}
