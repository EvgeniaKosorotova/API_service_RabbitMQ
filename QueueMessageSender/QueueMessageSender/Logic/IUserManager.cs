using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<bool> CreateAsync(string username, string password);
        Task<UserModel> GetAsync(int id = 0, string username = null, string password = null);
        Task<bool> DeleteAsync(int id);
    }
}
