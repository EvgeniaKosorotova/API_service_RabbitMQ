using QueueMessageSender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IUserManager
    {
        Task<UserModel> CreateAsync(string username, string password, string role);
        Task<UserModel> GetByIdAsync(int? id = null);
        Task<UserModel> GetByCredentialsAsync(string username = null, string password = null);
        Task<UserModel> GetByUsernameAsync(string username = null);
        Task DeleteAsync(int id);
        Task<List<UserModel>> GetAllAsync();
    }
}
