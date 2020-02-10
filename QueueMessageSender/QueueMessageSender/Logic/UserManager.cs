using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
using System;
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

        public UserManager(UserContext context)
        {
            db = context;
        }

        public async Task<bool> CreateAsync(string username, string password)
        {
            await db.Users.AddAsync(new UserModel
            {
                Username = username,
                Password = GetHash(password),
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
                return await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (token != null) 
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken.Equals(token));
                return user;
            }
            return null;
        }

        public async Task<bool> UpdateTokenAsync(string username, string refreshToken)
        {
            UserModel user = await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));
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

        public string GetHash(string str) 
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: str,
            salt: new byte[16],
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        }
    }
}
