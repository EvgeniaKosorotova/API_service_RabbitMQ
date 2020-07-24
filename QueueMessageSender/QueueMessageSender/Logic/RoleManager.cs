using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Data.Models;
using QueueMessageSender.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using RoleModel = QueueMessageSender.Models.RoleModel;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage role list.
    /// </summary>
    public class RoleManager : IRoleManager
    {
        private readonly DataContext db;

        public RoleManager(DataContext context)
        {
            db = context;
        }

        public async Task<RoleModel> AddAsync(string role) 
        {
            var roleObj = await db.Roles.AddAsync(new RoleObj
            {
                Role = role
            });
            await db.SaveChangesAsync();

            return new RoleModel { 
                Id = roleObj.Entity.Id,
                Role = roleObj.Entity.Role
            };
        }

        public async Task<RoleModel> GetAsync(string role)
        {
            var roleObj = await db.Roles.FirstOrDefaultAsync(r => r.Role.Equals(role));

            return new RoleModel
            {
                Id = roleObj.Id,
                Role = roleObj.Role
            };
        }

        public async Task<List<RoleModel>> GetAllAsync()
        {
            var roles = new List<RoleModel>();
            var rolesObj = await db.Roles.ToListAsync();

            foreach (RoleObj role in rolesObj)
            {
                roles.Add(new RoleModel {
                    Id = role.Id,
                    Role = role.Role
                });
            }
            return roles;
        }

        public async Task DeleteAsync(string role)
        {
            RoleObj roleObj = await db.Roles.FirstOrDefaultAsync(t => t.Role.Equals(role));

            if (roleObj != null)
            {
                db.Roles.Remove(roleObj);
            }

            await db.SaveChangesAsync();
        }
    }
}
