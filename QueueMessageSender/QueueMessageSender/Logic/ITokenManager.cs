using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface ITokenManager
    {
        Task<bool> AddTokenAsync(UserModel user, string refreshToken);
        Task<int?> GetUser(string token = "");
        Task<bool> DeleteAsync(string token);
        Task<bool> DeleteTokensAsync(int userId);
    }
}
