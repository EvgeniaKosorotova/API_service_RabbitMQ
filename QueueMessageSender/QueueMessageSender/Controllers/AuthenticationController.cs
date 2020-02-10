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
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly AuthenticationJWT _authenticationJWT;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IUserManager userManager,
                                  AuthenticationJWT authenticationJWT)
        {
            _userManager = userManager;
            _authenticationJWT = authenticationJWT;
        }

        /// <summary>
        /// Method of updating access tokens and refresh.
        /// </summary>
        //[AllowAnonymous]
        [HttpPost("refresh")]
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