using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
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

        public UserManager(DataContext context, HashGenerator hashGenerator)
        {
            db = context;
            _hashGenerator = hashGenerator;
        }

        public async Task<bool> CreateAsync(string username, string password)
        {
            await db.Users.AddAsync(new UserModel
            {
                Username = username,
                Password = _hashGenerator.GetHash(password)
            });

            return await db.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user != null)
            {
                db.Users.Remove(user);
            }

            return await db.SaveChangesAsync() > 0 ? true: false;
        }

        public async Task<UserModel> GetAsync(int? id = null, string username = null, string password = null)
        {
            if (id != 0)
            {
                return await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
            }

            if (username != null)
            {
                if (password != null)
                {
                    return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username) && u.Password.Equals(_hashGenerator.GetHash(password)));
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
