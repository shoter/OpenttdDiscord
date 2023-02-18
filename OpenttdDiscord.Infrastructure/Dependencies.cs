using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Database;
using OpenttdDiscord.Infrastructure.Guilds;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Servers;

namespace OpenttdDiscord.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection RegisterModules(this IServiceCollection services)
        {
            services.AddDbContextFactory<OttdContext>((IServiceProvider sp, DbContextOptionsBuilder builder) =>
            {
                string connectionString = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString;
                builder.UseNpgsql(connectionString, x =>
                {
                    x.MigrationsHistoryTable("__MigrationHistory");
                });
            });

            services
                .RegisterDependencies<ServersModule>()
                .RegisterDependencies<GuildsModule>();

            return services;
        }
    }
}
