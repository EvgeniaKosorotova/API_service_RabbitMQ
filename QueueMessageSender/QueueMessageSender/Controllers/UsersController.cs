using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly HashGenerator _hashGenerator;
        private readonly ITokenManager _tokenManager;

        public UsersController(IUserManager userManager, HashGenerator hashGenerator, ITokenManager tokenManager)
        {
            _userManager = userManager;
            _hashGenerator = hashGenerator;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AuthenticationModel authData)
        {
            UserModel user = await _userManager.GetAsync(username: authData.Username);

            if (user == null)
            {
                if (await _userManager.CreateAsync(authData.Username, authData.Password))
                {
                    return Created(string.Empty,
                        new
                        {
                            Message = "Credentials were recorded and saved."
                        });
                }
            }

            return BadRequest(
                new ErrorModel
                {
                    Error = "Credentials have been created earlier."
                });
        }

        /// <summary>
        /// Method to delete a record from the database.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int id)
        {
            UserModel user = await _userManager.GetAsync(id: id);

            if (user != null)
            {
                if (await _userManager.DeleteAsync(id)
                    && await _tokenManager.DeleteTokensAsync(userId: id))
                {
                    return Ok(
                        new MessageModel
                        {
                            Message = "Credentials have been deleted."
                        });
                }
            }

            return NotFound(
                new ErrorModel
                {
                    Error = "No credentials were found or deleted."
                });
        }
    }
}