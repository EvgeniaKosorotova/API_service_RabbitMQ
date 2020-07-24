using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Data.Models;
using QueueMessageSender.Models;
using System.Threading.Tasks;
using TokenModel = QueueMessageSender.Models.TokenModel;
using UserModel = QueueMessageSender.Models.UserModel;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage token list.
    /// </summary>
    public class TokenManager : ITokenManager
    {
        private readonly DataContext db;

        public TokenManager(DataContext context)
        {
            db = context;
        }

        public async Task<TokenModel> AddTokenAsync(UserModel user, string refreshToken) 
        {
            var tokens = await db.Tokens.AddAsync(new TokenObj
            {
                UserId = user.Id,
                RefreshToken = refreshToken
            });
            await db.SaveChangesAsync();

            return new TokenModel { 
                Id = tokens.Entity.Id,
                UserId = tokens.Entity.UserId,
                RefreshToken = tokens.Entity.RefreshToken,
            };
        }

        public async Task<UserModel> GetUser(string token = "")
        {
            var tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));

            return tokenObj == null ? null : 
                new UserModel {
                    Id = tokenObj.User.Id,
                    Username = tokenObj.User.Username,
                    Password = tokenObj.User.Password,
                    RoleId = tokenObj.User.RoleId
                };
        }

        public async Task DeleteAsync(string token)
        {
            TokenObj tokenObj = await db.Tokens.FirstOrDefaultAsync(t => t.RefreshToken.Equals(token));

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
