using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QueueMessageSender.Controllers.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthJWTController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthJWTController(IConfiguration configuration,
                                SignInManager<IdentityUser> signInManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!result.Succeeded) 
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtExpiryInMinutes"]));
            //var expiryDays = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
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