using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System;

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
        [HttpGet]
        public IActionResult Login(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            if (!(user != null && user.Username == authData.Username && user.Password == _userManager.GetHashPassword(authData.Password)))
            {
                return BadRequest(
                    new AuthenticationResultModel
                    {
                        Error = "Username and password are invalid."
                    });
            }
            var accessToken = _authenticationJWT.CreateAccessToken(user.Username);
            var refreshToken = _authenticationJWT.CreateRefreshToken();
            if (_userManager.UpdateTokenAsync(user.Username, refreshToken).Result)
            {
                return Ok(
                    new AuthenticationResultModel
                    {
                        AccessToken = accessToken,
                        LifeTime = $"{TimeSpan.FromMinutes(_configuration.GetValue<double>("Settings:JWT:AccessToken:ExpiryInMinutes")).TotalSeconds} sec",
                        RefreshToken = refreshToken
                    });
            }
            return BadRequest(
                new AuthenticationResultModel
                {
                    Error = "Tokens have not been created."
                });
        }

        /// <summary>
        /// Method of updating access tokens and refresh.
        /// </summary>
        [HttpPut]
        public IActionResult Refresh(string oldRefreshToken)
        {
            UserModel user = _userManager.GetAsync(token: oldRefreshToken).Result;
            if (user != null) 
            {
                var accessToken = _authenticationJWT.CreateAccessToken(user.Username);
                var refreshToken = _authenticationJWT.CreateRefreshToken();
                if (_userManager.UpdateTokenAsync(user.Username, refreshToken).Result)
                {
                    return Ok(
                        new AuthenticationResultModel
                        {
                            AccessToken = accessToken,
                            LifeTime = $"{TimeSpan.FromMinutes(_configuration.GetValue<double>("Settings:JWT:AccessToken:ExpiryInMinutes")).TotalSeconds} sec",
                            RefreshToken = refreshToken
                        });
                }
                return BadRequest(
                    new AuthenticationResultModel
                    {
                        Error = "Tokens have not been created."
                    });
            }

            return BadRequest(
                    new AuthenticationResultModel
                    {
                        Error = "Token is invalid."
                    });
        }
    }
}