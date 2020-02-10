using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly AuthenticationJWT _authenticationJWT;
        private readonly IConfiguration _configuration;

        public UserController(IUserManager userManager,
                                AuthenticationJWT authenticationJWT,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _authenticationJWT = authenticationJWT;
            _configuration = configuration;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        //[AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;

            if (!(user != null && user.Username == authData.Username && user.Password == authData.Password))
                if (_userManager.CreateAsync(authData.Username, authData.Password).Result)
                {
                    return Ok(
                        new AuthenticationResultModel
                        {
                            Error = "Credentials were recorded and saved."
                        });
                }
            return BadRequest(
                new AuthenticationResultModel
                {
                    Error = "Credentials have been created earlier."
                });
        }

        /// <summary>
        /// A post method that checks the accepted credentials.
        /// If successful, creates a token for authentication.
        /// </summary>
        //[AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            if (!(user != null && user.Username == authData.Username && user.Password == authData.Password))
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
        /// Method to delete a record from the database.
        /// </summary>
        [HttpPost("delete")]
        public IActionResult Delete(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            if (user != null && user.Username == authData.Username && user.Password == authData.Password)
                if (_userManager.DeleteAsync(authData.Username).Result)
                {
                    return Ok(
                        new AuthenticationResultModel
                        {
                            Error = "Credentials have been deleted."
                        });
                }
            return BadRequest(
                new AuthenticationResultModel
                {
                    Error = "Credentials were not deleted."
                });
        }
    }
}