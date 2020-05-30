using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage role list.
    /// </summary>
    public class RoleManager : IRoleManager
    {
        private static DataContext db;

        public RoleManager(DataContext context)
        {
            db = context;
        }

        public async Task<RoleModel> AddAsync(string role) 
        {
            var roleObj = await db.Roles.AddAsync(new RoleModel
            {
                Role = role
            });
            await db.SaveChangesAsync();

            return roleObj.Entity;
        }

        public async Task<RoleModel> GetAsync(string role)
        {
            return await db.Roles.FirstOrDefaultAsync(r => r.Role.Equals(role));
        }

        public async Task<List<RoleModel>> GetAllAsync()
        {
            return await db.Roles.ToListAsync();
        }

        public async Task DeleteAsync(string role)
        {
            RoleModel roleObj = await db.Roles.FirstOrDefaultAsync(t => t.Role.Equals(role));

            if (roleObj != null)
            {
                db.Roles.Remove(roleObj);
            }

            await db.SaveChangesAsync();
        }
    }
}
