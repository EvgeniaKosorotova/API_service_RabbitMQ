using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace QueueMessageSender.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<TokenModel> Tokens { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
