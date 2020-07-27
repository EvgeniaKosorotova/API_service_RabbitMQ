using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Data.Models;
using QueueMessageSender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserModel = QueueMessageSender.Models.UserModel;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to manage user list.
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly DataContext db;
        private readonly HashGenerator _hashGenerator;
        private readonly IRoleManager _roleManager;

        public UserManager(DataContext context, HashGenerator hashGenerator, IRoleManager roleManager)
        {
            db = context;
            _hashGenerator = hashGenerator;
            _roleManager = roleManager;
        }

        public async Task<UserModel> CreateAsync(string username, string password, int roleId)
        {
            var user = await db.Users.AddAsync(new UserObj
            {
                Username = username,
                Password = _hashGenerator.GetHash(password),
                RoleId = roleId
            });

            await db.SaveChangesAsync();

            return new UserModel {
                Id = user.Entity.Id,
                Username = user.Entity.Username,
                Password = user.Entity.Password,
                RoleId = user.Entity.RoleId
            };
        }

        public async Task DeleteAsync(int id)
        {
            UserObj user = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user != null)
            {
                db.Users.Remove(user);
            }

            await db.SaveChangesAsync();
        }

        public async Task<UserModel> GetByIdAsync(int? id = null)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            return new UserModel { 
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                RoleId = user.RoleId
            };
        }

        public async Task<UserModel> GetByCredentialsAsync(string username = null, string password = null)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username) && u.Password.Equals(_hashGenerator.GetHash(password)));

            return new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                RoleId = user.RoleId
            };
        }

        public async Task<UserModel> GetByUsernameAsync(string username = null)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username.Equals(username));

            return new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                RoleId = user.RoleId
            };
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var users = new List<UserModel>();
            var usersObj = await db.Users.ToListAsync();

            foreach (UserObj user in usersObj)
            {
                users.Add(new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password,
                    RoleId = user.RoleId
                });
            }
            return users;
        }

        public async Task UpdateAsync(UserModel userOld, UserModel user)
        {
            db.Users.Update(new UserObj {
                Id = userOld.Id,
                Username = user.Username,
                RoleId = user.RoleId,
                Password = user.Password,
            });
            await db.SaveChangesAsync();
        }
    }
}
