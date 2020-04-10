using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager : IUserManager
    {
        private static DataContext db;
        private readonly Helper _helper;

        public UserManager(DataContext context, Helper helper)
        {
            db = context;
            _helper = helper;
        }

        public async Task<int> CreateAsync(string username, string password)
        {
            await db.Users.AddAsync(new UserModel
            {
                Username = username,
                Password = _helper.GetHash(password)
            });

            return await db.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user != null)
            {
                db.Users.Remove(user);
            }

            return await db.SaveChangesAsync();
        }

        public async Task<UserModel> GetAsync(int id = 0, string username = null, string password = null)
        {
            if (id != 0)
            {
                return await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
            }

            if (username != null)
            {
                if (password != null)
                {
                    return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username) && u.Password.Equals(_helper.GetHash(password)));
                }
                else
                {
                    return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));
                }
            }

            return null;
        }
    }
}
