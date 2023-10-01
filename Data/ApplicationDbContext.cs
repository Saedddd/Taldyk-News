using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaldykNews.Models;
using TaldykNews.WebUI.Models;

namespace TaldykNews.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = 1, Name = "Admin" };
            Role writerRole = new Role { Id = 2, Name = "Writer" };
            User ownerUser = new User
            {
                Id = 1,
                Email = WebConstants.DefaultAdminEmail,
                Password = WebConstants.DefaultAdminPassword,
                FullName = WebConstants.DefaultAdminFullName,
                RoleId = adminRole.Id
            };
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, writerRole });
            modelBuilder.Entity<User>().HasData(new User[] { ownerUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
