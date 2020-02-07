using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;

namespace QueueMessageSender.Logic
{
    public class UserContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options) :
            base(options){}
    }
}
