using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenttdDiscord.Database;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.EventLogs;
using OpenttdDiscord.Infrastructure.Guilds;
using OpenttdDiscord.Infrastructure.Maintenance;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Ottd;
using OpenttdDiscord.Infrastructure.Rcon;
using OpenttdDiscord.Infrastructure.Reporting;
using OpenttdDiscord.Infrastructure.Roles;
using OpenttdDiscord.Infrastructure.Servers;
using OpenttdDiscord.Infrastructure.Statuses;

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

            services.AddSingleton<IAkkaService, AkkaService>();

            services
                .RegisterDependencies<ServersModule>()
                .RegisterDependencies<GuildsModule>()
                .RegisterDependencies<OttdModule>()
                .RegisterDependencies<DiscordModule>()
                .RegisterDependencies<StatusesModule>()
                .RegisterDependencies<ChattingModule>()
                .RegisterDependencies<RconModule>()
                .RegisterDependencies<EventLogModule>()
                .RegisterDependencies<ReportingModule>()
                .RegisterDependencies<MaintenanceModule>()
                .RegisterDependencies<RolesModule>();

            return services;
        }
    }
}
