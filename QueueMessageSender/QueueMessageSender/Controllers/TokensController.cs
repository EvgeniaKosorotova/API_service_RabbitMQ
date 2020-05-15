using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;
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
        private readonly ServiceTokens _serviceTokens;

        public TokensController(IUserManager userManager,
                                  AuthenticationJWT authenticationJWT,
                                  IConfiguration configuration,
                                  ITokenManager tokenManager,
                                  ServiceTokens serviceTokens)
        {
            _userManager = userManager;
            _authenticationJWT = authenticationJWT;
            _configuration = configuration;
            _tokenManager = tokenManager;
            _serviceTokens = serviceTokens;
        }

        /// <summary>
        /// A post method that checks the accepted credentials.
        /// If successful, creates a token for authentication.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LoginAsync(AuthenticationModel authData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Incoming data is not valid."
                });
            }

            UserModel user = await _userManager.GetByCredentialsAsync(username: authData.Username, password: authData.Password);

            if (user == null)
            {
                return BadRequest(
                    new ErrorModel
                    {
                        Error = "Username and password are invalid."
                    });
            }

            var authenticationResult = await _serviceTokens.CreateTokensAsync(user);

            return Ok(authenticationResult);
        }

        /// <summary>
        /// Method of updating access tokens and refresh.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> RefreshAsync([FromQuery]string refreshToken)
        {
            var user = await _tokenManager.GetUser(token: refreshToken);

            if (user == null) 
            {
                return BadRequest(
                    new ErrorModel
                    {
                        Error = "Token is invalid."
                    });
            }
            await _tokenManager.DeleteAsync(refreshToken);

            var authenticationResult = await _serviceTokens.CreateTokensAsync(user);

            return Ok(authenticationResult);
        }
    }
}