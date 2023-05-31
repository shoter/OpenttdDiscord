using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenttdDiscord.Infrastructure.Maintenance.HealthChecks;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Maintenance
{
    public class MaintenanceModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthcheck>("Db");
            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(5);
                options.Period = TimeSpan.FromSeconds(20);
            });
        }
    }
}