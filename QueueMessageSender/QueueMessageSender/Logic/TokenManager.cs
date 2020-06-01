using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage token list.
    /// </summary>
    public class TokenManager : ITokenManager
    {
        private static DataContext db;

        public TokenManager(DataContext context)
        {
            db = context;
        }

        public async Task<TokenModel> AddTokenAsync(UserModel user, string refreshToken) 
        {
            var tokens = await db.Tokens.AddAsync(new TokenModel
            {
                UserId = user.Id,
                User = user,
                RefreshToken = refreshToken
            });
            await db.SaveChangesAsync();

            return tokens.Entity;
        }

        public async Task<UserModel> GetUser(string token = "")
        {
            var tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));

            return tokenObj?.User;
        }

        public async Task DeleteAsync(string token)
        {
            TokenModel tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));

            if (tokenObj != null)
            {
                db.Tokens.Remove(tokenObj);
            }

            await db.SaveChangesAsync();
        }

        public async Task DeleteTokensAsync(int userId)
        {
            var tokenObj = await db.Tokens.FindAsync(userId);

            if (tokenObj != null)
            {
                db.Tokens.RemoveRange(tokenObj);
            }

            await db.SaveChangesAsync();
        }
    }
}
