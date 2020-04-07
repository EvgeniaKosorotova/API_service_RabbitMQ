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

        public TokensController(IUserManager userManager,
                                  AuthenticationJWT authenticationJWT,
                                  IConfiguration configuration)
        {
            _userManager = userManager;
            _authenticationJWT = authenticationJWT;
            _configuration = configuration;
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
            if (await _userManager.UpdateTokenAsync(user.Username, refreshToken))
            {
                return Ok(
                    new AuthenticationResultModel
                    {
                        AccessToken = accessToken,
                        LifeTime = TimeSpan.ParseExact(_configuration.GetSection("Settings:JWT:AccessToken:Expiry").Value, "c", null),
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
            UserModel user = await _userManager.GetAsync(token: refreshToken);
            if (user == null) 
            {
                return BadRequest(
                    new ErrorModel
                    {
                        Error = "Token is invalid."
                    });
            }

            var newAccessToken = _authenticationJWT.CreateAccessToken(user.Username);
            var newRefreshToken = _authenticationJWT.CreateRefreshToken();

            if (await _userManager.UpdateTokenAsync(user.Username, refreshToken))
            {
                return Ok(
                    new AuthenticationResultModel
                    {
                        AccessToken = newAccessToken,
                        LifeTime = TimeSpan.ParseExact(_configuration.GetSection("Settings:JWT:AccessToken:Expiry").Value, "c", null),
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