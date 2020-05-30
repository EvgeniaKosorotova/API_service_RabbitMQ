using QueueMessageSender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public interface IRoleManager
    {
        Task<RoleModel> AddAsync(string role);
        Task<RoleModel> GetAsync(string role);
        Task<List<RoleModel>> GetAllAsync();
        Task DeleteAsync(string role);
    }
}
