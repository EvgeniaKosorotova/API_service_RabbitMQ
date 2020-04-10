using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<int> CreateAsync(string username, string password);
        Task<UserModel> GetAsync(int id = 0, string username = null, string password = null);
        Task<int> DeleteAsync(int id);
    }
}
