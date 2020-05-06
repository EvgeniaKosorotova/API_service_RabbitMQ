using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface ITokenManager
    {
        Task<TokenModel> AddTokenAsync(UserModel user, string refreshToken);
        Task<UserModel> GetUser(string token = "");
        Task DeleteAsync(string token);
        Task DeleteTokensAsync(int userId);
    }
}
