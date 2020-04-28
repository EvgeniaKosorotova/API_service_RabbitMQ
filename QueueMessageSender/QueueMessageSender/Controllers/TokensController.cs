using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TokensController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly AuthenticationJWT _authenticationJWT;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;

        public TokensController(IUserManager userManager,
                                  AuthenticationJWT authenticationJWT,
                                  IConfiguration configuration,
                                  ITokenManager tokenManager)
        {
            _userManager = userManager;
            _authenticationJWT = authenticationJWT;
            _configuration = configuration;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// A post method that checks the accepted credentials.
        /// If successful, creates a token for authentication.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LoginAsync(AuthenticationModel authData)
        {
            UserModel user = await _userManager.GetAsync(username: authData.Username, password: authData.Password);

            if (user == null)
            {
                return BadRequest(
                    new ErrorModel
                    {
                        Error = "Username and password are invalid."
                    });
            }

            var accessToken = _authenticationJWT.CreateAccessToken(user.Username);
            var refreshToken = _authenticationJWT.CreateRefreshToken();

            if (await _tokenManager.AddTokenAsync(user, refreshToken))
            {
                return Ok(
                    new AuthenticationResultModel
                    {
                        AccessToken = accessToken,
                        LifeTime = _configuration.GetValue<TimeSpan>("Settings:JWT:AccessToken:Expiry"),
                        RefreshToken = refreshToken
                    });
            }

            return BadRequest(
                new ErrorModel
                {
                    Error = "Tokens have not been created."
                });
        }

        /// <summary>
        /// Method of updating access tokens and refresh.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> RefreshAsync(string refreshToken)
        {
            var userId = await _tokenManager.GetUser(token: refreshToken);

            if (userId <= 0) 
            {
                return BadRequest(
                    new ErrorModel
                    {
                        Error = "Token is invalid."
                    });
            }

            var user = await _userManager.GetAsync(id: userId);
            var newAccessToken = _authenticationJWT.CreateAccessToken(user.Username);
            var newRefreshToken = _authenticationJWT.CreateRefreshToken();

            if (await _tokenManager.DeleteAsync(refreshToken) 
                && await _tokenManager.AddTokenAsync(user, newRefreshToken))
            {
                return Ok(
                    new AuthenticationResultModel
                    {
                        AccessToken = newAccessToken,
                        LifeTime = _configuration.GetValue<TimeSpan>("Settings:JWT:AccessToken:Expiry"),
                        RefreshToken = newRefreshToken
                    });
            }

            return BadRequest(
                new ErrorModel
                {
                    Error = "Tokens have not been created."
                });
        }
    }
}