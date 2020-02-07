using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;

namespace QueueMessageSender.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IQueueMessageSender _sender;
        private readonly UserManager _userManager;
        private readonly AuthenticationJWT _authenticationJWT;

        public MessagesController(ILogger<MessagesController> logger,
                                  IQueueMessageSender sender,
                                  UserContext context)
        {
            _logger = logger;
            _sender = sender;
            _authenticationJWT = AuthenticationJWT.Instance;
            _userManager = UserManager.Instance;
            _userManager.Context = context;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            _logger.LogInformation($"Register method. Username: {authData.Username}, Password: {authData.Password}");
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!(user != null && user.Username == authData.Username && user.Password == authData.Password)) //|| !result.Succeeded
                if (_userManager.CreateAsync(authData.Username, authData.Password).Result) 
                {
                    _logger.LogInformation($"GoodRequest: Username and password are invalid. Username: {authData.Username}, Password: {authData.Password}");

                    return Ok(
                        new AuthenticationResultModel
                        {
                            Successful = true,
                            Error = "Credentials were recorded and saved."
                        });
                }
            _logger.LogInformation($"BadRequest: Credentials have been created earlier. Username: {authData.Username}, Password: {authData.Password}");

            return BadRequest(
                new AuthenticationResultModel
                {
                    Successful = false,
                    Error = "Credentials have been created earlier."
                });
        }

        /// <summary>
        /// A post method that checks the accepted credentials.
        /// If successful, creates a token for authentication.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            _logger.LogInformation($"Login method. Username: {authData.Username}, Password: {authData.Password}");
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (!(user != null && user.Username == authData.Username && user.Password == authData.Password)) //|| !result.Succeeded
            {
                _logger.LogInformation($"BadRequest: Username and password are invalid. Username: {authData.Username}, Password: {authData.Password}");

                return BadRequest(
                    new AuthenticationResultModel
                    {
                        Successful = false,
                        Error = "Username and password are invalid."
                    });
            }

            return CreateTokens(user);
        }

        /// <summary>
        /// Method of updating access tokens and refresh.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh(string oldRefreshToken)
        {
            UserModel user = _userManager.GetAsync(token: oldRefreshToken).Result;
            if (user != null) 
                return CreateTokens(user);

            return BadRequest(
                    new AuthenticationResultModel
                    {
                        Successful = false,
                        Error = "Token is invalid."
                    });
        }

        /// <summary>
        /// A method that accepts JSON with message information and sends the message in exchange.
        /// </summary>
        [HttpPost("send")]
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

        /// <summary>
        /// Method to delete a record from the database.
        /// </summary>
        [HttpPost("delete")]
        public IActionResult Delete(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            _logger.LogInformation($"Delete method. Username: {authData.Username}, Password: {authData.Password}");
            //var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (user != null && user.Username == authData.Username && user.Password == authData.Password) //|| !result.Succeeded
                if (_userManager.DeleteAsync(authData.Username).Result)
                {
                    _logger.LogInformation($"GoodRequest: Credentials have been deleted. Username: {authData.Username}, Password: {authData.Password}");

                    return Ok(
                        new AuthenticationResultModel
                        {
                            Successful = false,
                            Error = "Credentials have been deleted."
                        });
                }
            _logger.LogInformation($"BadRequest: Credentials were not deleted. Username: {authData.Username}, Password: {authData.Password}");

            return BadRequest(
                new AuthenticationResultModel
                {
                    Successful = false,
                    Error = "Credentials were not deleted."
                });
        }

        /// <summary>
        /// Token creation method.
        /// </summary>
        private IActionResult CreateTokens(UserModel user) 
        {
            var tokens = _authenticationJWT.CreateTokens(user.Username);
            if (tokens.TryGetValue("accessToken", out string accessToken) && tokens.TryGetValue("refreshToken", out string refreshToken))
                if (_userManager.UpdateTokenAsync(user.Username, refreshToken).Result)
                {
                    _logger.LogInformation($"GoodRequest. Username: {user.Username}, Password: {user.Password}, Access token: {accessToken}, Refresh token: {refreshToken}");

                    return Ok(
                        new AuthenticationResultModel
                        {
                            Successful = true,
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        });
                }
            _logger.LogInformation($"BadRequest: Tokens have not been created. Username: {user.Username}, Password: {user.Password}");
                
            return BadRequest(
                new AuthenticationResultModel
                {
                    Successful = false,
                    Error = "Tokens have not been created."
                });
        }
    }
}