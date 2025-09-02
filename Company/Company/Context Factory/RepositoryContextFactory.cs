using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Repository;
namespace Company.Context_Factory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var configurations = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configurations.GetConnectionString("sqlConnection");
            var builder = new DbContextOptionsBuilder<Context>()
            .UseSqlServer(connectionString,b => b.MigrationsAssembly("Company"));
            return new Context(builder.Options);
        }
    }

}
