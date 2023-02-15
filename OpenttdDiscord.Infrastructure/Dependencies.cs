using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Servers;

namespace OpenttdDiscord.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection RegisterModules(this IServiceCollection services)
        {
            services.RegisterDependencies<ServersModule>();

            return services;
        }
    }
}
