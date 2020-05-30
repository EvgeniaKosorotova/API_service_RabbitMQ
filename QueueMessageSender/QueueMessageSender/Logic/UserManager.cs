using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager : IUserManager
    {
        private static DataContext db;
        private readonly HashGenerator _hashGenerator;
        private readonly IRoleManager _roleManager;

        public UserManager(DataContext context, HashGenerator hashGenerator, IRoleManager roleManager)
        {
            db = context;
            _hashGenerator = hashGenerator;
            _roleManager = roleManager;
        }

        public async Task<UserModel> CreateAsync(string username, string password, string role)
        {
            RoleModel roleObj = await _roleManager.GetAsync(role);

            if (roleObj == null)
            {
                roleObj = await _roleManager.AddAsync(role);
            }

            var user = await db.Users.AddAsync(new UserModel
            {
                Username = username,
                Password = _hashGenerator.GetHash(password),
                RoleId = roleObj.Id
            });

            await db.SaveChangesAsync();

            return user.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user != null)
            {
                db.Users.Remove(user);
            }

            await db.SaveChangesAsync();
        }

        public async Task<UserModel> GetByIdAsync(int? id = null)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        }

        public async Task<UserModel> GetByCredentialsAsync(string username = null, string password = null)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username) && u.Password.Equals(_hashGenerator.GetHash(password)));
        }

        public async Task<UserModel> GetByUsernameAsync(string username = null)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));
        }
    }
}
