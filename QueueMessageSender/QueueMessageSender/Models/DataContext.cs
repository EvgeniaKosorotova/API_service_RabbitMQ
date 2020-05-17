using Microsoft.EntityFrameworkCore;

namespace QueueMessageSender.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TokenModel> Tokens { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TokenModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.IdUser).HasColumnName("IdUser");

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasColumnName("RefreshToken")
                    .HasMaxLength(250); ;

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Tokens_Users");
            });

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("Password")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("Username")
                    .HasMaxLength(50);
            });
        }
    }
}
