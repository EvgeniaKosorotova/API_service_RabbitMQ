using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<bool> CreateAsync(string username, string password);
        Task<UserModel> GetAsync(string username = null, string token = null);
        Task<bool> UpdateTokenAsync(string username, string refreshToken);
        Task<bool> DeleteAsync(string username);
        Task<bool> SaveAsync();
        string GetHash(string password);
    }
}
