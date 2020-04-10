using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface ITokenManager
    {
        Task<int> AddTokenAsync(UserModel user, string refreshToken);
        Task<int> GetUser(string token = "");
        Task<int> DeleteAsync(string token);
        Task<int> DeleteTokensAsync(int userId);
    }
}
