using Microsoft.EntityFrameworkCore;
using SecureNoteTakingApi.Models;

namespace SecureNoteTakingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Users table
        public DbSet<User> Users { get; set; }

        //Notes table
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Username must be unique in the database
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //One user has many notes and when deleting user it also deletes their notes
            modelBuilder.Entity<User>()
                .HasMany(u => u.Notes)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}