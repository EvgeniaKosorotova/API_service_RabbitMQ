using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QueueMessageSender.Controllers.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthJWTController : ControllerBase
    {
        private IConfiguration _configuration;
        //private readonly SignInManager<IdentityUser> _signInManager;

        public AuthJWTController(IConfiguration configuration
            //, SignInManager<IdentityUser> signInManager
            )
        {
            _configuration = configuration;
            //_signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);
            var username = _configuration.GetValue<string>("Settings:Credentials:UserName");
            var password = _configuration.GetValue<string>("Settings:Credentials:Password");

            if (!(username == login.Username && password == login.Password)) //|| !result.Succeeded
            {
                return BadRequest(
                    new LoginResultModel
                    {
                        Successful = false,
                        Error = "Username and password are invalid."
                    });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Username)
            };

            var JwtSecurityKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Settings:JWT:SecurityKey"));
            var key = new SymmetricSecurityKey(JwtSecurityKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Settings:JWT:ExpiryInMinutes"));

            var token = new JwtSecurityToken(
                _configuration.GetValue<string>("Settings:JWT:Issuer"),
                _configuration.GetValue<string>("Settings:JWT:Audience"),
                claims,
                expires: expiryMinutes,
                signingCredentials: creds
                );

            return Ok(
                new LoginResultModel
                {
                    Successful = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
        }
    }
}