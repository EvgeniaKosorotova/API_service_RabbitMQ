using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<UserModel> CreateAsync(string username, string password);
        Task<UserModel> GetByIdAsync(int? id = null);
        Task<UserModel> GetByCredentialsAsync(string username = null, string password = null);
        Task<UserModel> GetByUsernameAsync(string username = null);
        Task DeleteAsync(int id);
    }
}
