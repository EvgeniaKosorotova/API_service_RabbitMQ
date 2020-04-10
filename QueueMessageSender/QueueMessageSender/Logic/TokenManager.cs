using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
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

        public async Task<int> AddTokenAsync(UserModel user, string refreshToken) 
        {
            await db.Tokens.AddAsync(new TokenModel
            {
                IdUser = user.Id,
                RefreshToken = refreshToken
            });

            return await db.SaveChangesAsync();
        }

        public async Task<int> GetUser(string token = "")
        {
            var tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));

            return tokenObj.IdUser;
        }

        public async Task<int> DeleteAsync(string token)
        {
            TokenModel tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));
            if (tokenObj != null)
            {
                db.Tokens.Remove(tokenObj);
            }
            return await db.SaveChangesAsync();
        }

        public async Task<int> DeleteTokensAsync(int userId)
        {
            var tokenObj = await db.Tokens.FindAsync(userId);
            if (tokenObj != null)
            {
                db.Tokens.RemoveRange(tokenObj);
            }
            return await db.SaveChangesAsync();
        }
    }
}
