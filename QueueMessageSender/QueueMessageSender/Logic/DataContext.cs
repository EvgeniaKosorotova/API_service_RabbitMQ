using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;
using QueueMessageSender.Models;

namespace QueueMessageSender.Logic
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }
        public DataContext(DbContextOptions<DataContext> options) :
            base(options){}
    }
}
