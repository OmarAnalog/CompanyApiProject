using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using Repository.Seed;

namespace Repository
{
    public class Context : IdentityDbContext<User>
    {
        // Define DbSets for your entities here
        public DbSet<Company>?  Companies{ get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public Context(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CompanySeed());
            modelBuilder.ApplyConfiguration(new EmployeeSeed());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
