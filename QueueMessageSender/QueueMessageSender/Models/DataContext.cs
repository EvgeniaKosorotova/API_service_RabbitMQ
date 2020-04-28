using Microsoft.EntityFrameworkCore;
using QueueMessageSender.Logic.Models;

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

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=dbusers;Integrated Security=True");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TokenModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasColumnName("refreshToken");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Tokens_Users");
            });

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
