using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QueueMessageSender.Controllers
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessagesController> _logger;
        private readonly IQueueMessageSender _sender;

        public MessagesController(IConfiguration configuration, IQueueMessageSender sender, ILogger<MessagesController> logger)
        {
            _configuration = configuration;
            _sender = sender;
            _logger = logger;
        }


        /// <summary>
        /// A post method that checks the accepted credentials.
        /// If successful, creates a token for authentication.
        /// </summary>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult Login(AuthenticationModel login)
        {
            _logger.LogInformation($"Login method. Username: {login.Username}, Password: {login.Password}");
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);
            var username = _configuration.GetValue<string>("Settings:Credentials:UserName");
            var password = _configuration.GetValue<string>("Settings:Credentials:Password");

            if (!(username == login.Username && password == login.Password)) //|| !result.Succeeded
            {
                _logger.LogInformation($"BadRequest. Username: {login.Username}, Password: {login.Password}");
                return BadRequest(
                    new AuthenticationResultModel
                    {
                        Successful = false,
                        Error = "Username and password are invalid."
                    });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Username)
            };
            var jwtSecurityKey = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Settings:JWT:SecurityKey"));
            var key = new SymmetricSecurityKey(jwtSecurityKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Settings:JWT:ExpiryInMinutes"));

            var token = new JwtSecurityToken(
                _configuration.GetValue<string>("Settings:JWT:Issuer"),
                _configuration.GetValue<string>("Settings:JWT:Audience"),
                claims,
                expires: expiryMinutes,
                signingCredentials: creds
                );
            var securityToken = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation($"GoodRequest. Username: {login.Username}, Password: {login.Password}, Token: {securityToken}");
            return Ok(
                new AuthenticationResultModel
                {
                    Successful = true,
                    Token = securityToken
                });
        }

        /// <summary>
        /// A method that accepts JSON with message information and sends the message in exchange.
        /// </summary>
        [Route("send")]
        [HttpPost]
        public IActionResult Send(ReceivedDataModel model)
        {
            var departureData = new DepartureDatаRMQModel
            {
                NameExchange = model.Exchange,
                RoutingKey = model.Key,
                Message = model.Message
            };
            _logger.LogInformation($"Post method. Exchange: {departureData.NameExchange}, routing key: {departureData.RoutingKey}, message: {departureData.Message}");
            _sender.SendMessage(departureData);

            return Ok();
        }
    }
}