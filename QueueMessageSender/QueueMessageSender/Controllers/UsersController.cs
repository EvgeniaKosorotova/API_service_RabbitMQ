using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ITokenManager _tokenManager;

        public UsersController(IUserManager userManager, ITokenManager tokenManager)
        {
            _userManager = userManager;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegistrationModel registData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Incoming data is not valid."
                });
            }

            UserModel user = await _userManager.GetByUsernameAsync(username: registData.Username);

            if (user == null)
            {
                UserModel userNew = await _userManager.CreateAsync(registData.Username, registData.Password, registData.Role);

                return Created(string.Empty, userNew);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Incoming data is not valid."
                });
            }

            UserModel user = await _userManager.GetByIdAsync(id: id);

            if (user != null)
            {
                await _userManager.DeleteAsync(id);
                await _tokenManager.DeleteTokensAsync(userId: id);

                return Ok();
            }

            return StatusCode(405);
        }

        /// <summary>
        /// A method of getting list users from the database.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var users = await _userManager.GetAllAsync();

            return Ok(
                new UsersModel<UserModel>
                {
                    Result = users
                });
        }

        /// <summary>
        /// Method of updating user.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UserModel user)
        {
            UserModel userObj = await _userManager.GetByIdAsync(id: user.Id);

            if (userObj != null)
            {
                await _userManager.UpdateAsync(userObj, user);

                return Ok();
            }

            return BadRequest();
        }
    }
}