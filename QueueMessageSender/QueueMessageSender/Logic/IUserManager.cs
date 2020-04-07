using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<bool> CreateAsync(string username, string password);
        Task<UserModel> GetAsync(int id = 0, string username = null, string password = null, string token = null);
        Task<bool> UpdateTokenAsync(string username, string refreshToken);
        Task<bool> DeleteAsync(int id);
        Task<bool> SaveAsync();
    }
}
