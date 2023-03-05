using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace OpenttdDiscord.Database.Migrator
{
    internal static class MigratorBuilder
    {
        public static IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging()
                .RegisterDependencies();
        }

        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.MinimumLevel.Verbose()
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });
        }

        public static IHostBuilder RegisterDependencies(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((HostBuilderContext context, IServiceCollection services) =>
            {
                services.AddDbContextFactory<OttdContext>((IServiceProvider sp, DbContextOptionsBuilder builder) =>
                {
                    string connectionString = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString;
                    builder.UseNpgsql(connectionString, x =>
                    {
                        x.MigrationsHistoryTable("__MigrationHistory");
                    });
                }).ConfigureOptions(context);
            });
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, HostBuilderContext context)
        {
            return services
                .Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
        }
    }
}
