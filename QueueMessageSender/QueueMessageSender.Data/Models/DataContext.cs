using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace QueueMessageSender.Data.Models
{
    public partial class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<TokenObj> Tokens { get; set; }
        public DbSet<UserObj> Users { get; set; }
        public DbSet<RoleObj> Roles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
