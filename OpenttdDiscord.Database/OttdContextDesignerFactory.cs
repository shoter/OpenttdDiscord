using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OpenttdDiscord.Database
{
    internal class OttdContextDesignerFactory : IDesignTimeDbContextFactory<OttdContext>
    {
        public OttdContext CreateDbContext(string[] args)
        {
            ConfigurationBuilder configBuilder = new();
            configBuilder.AddUserSecrets(typeof(OttdContextDesignerFactory).Assembly);
            var config = configBuilder.Build();

            string connectionString = config["ConnectionString"] ?? throw new NullReferenceException();

            var optionsBuilder = new DbContextOptionsBuilder<OttdContext>();
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable(connectionString), x =>
            {
                x.MigrationsHistoryTable("__MigrationHistory");
            });

            return new OttdContext(optionsBuilder.Options);
        }
    }
}
