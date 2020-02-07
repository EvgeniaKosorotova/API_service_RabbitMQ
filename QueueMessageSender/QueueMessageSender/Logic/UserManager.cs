using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager : IUserManager
    {
        private static UserContext db;
        private readonly string defaultRefreshToken = "";
        private static UserManager _instance;

        public static UserManager Instance
        {
            get
            {
                if (_instance == null) 
                {
                    _instance = new UserManager();
                }
                return _instance;
            }
        }

        public UserContext Context
        {
            set => db = value;
        }

        private UserManager(){}

        public async Task<bool> CreateAsync(string username, string password)
        {
            await db.Users.AddAsync(new UserModel
            {
                Username = username,
                Password = password,
                RefreshToken = defaultRefreshToken
            });
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(string username)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                db.Users.Remove(user);
            }
            return await SaveAsync();
        }

        public async Task<UserModel> GetAsync(string username = null, string token = null)
        {
            if (username != null) 
                return await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (token != null) 
            {
                var user = (from u in db.Users.AsQueryable()
                             where u.RefreshToken == token
                             select u).SingleOrDefault<UserModel>();
                //var user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
                Console.WriteLine($"{db.Users.FirstOrDefault().Username}, {db.Users.FirstOrDefault().Password}, {db.Users.FirstOrDefault().RefreshToken}");
                return user;
            }
                //return await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
            return null;
        }

        public async Task<bool> UpdateTokenAsync(string username, string refreshToken)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && refreshToken != defaultRefreshToken)
            {
                var entity = db.Users.Attach(user);
                entity.State = EntityState.Modified;
                user.RefreshToken = refreshToken;
            }
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            if (await db.SaveChangesAsync() > 0) 
            {
                return true;
            }
            return false;
        }
    }
}
