using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database;
using OpenttdDiscord.Infrastructure.Modularity;
using OpenttdDiscord.Infrastructure.Servers;

namespace OpenttdDiscord.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection RegisterModules(this IServiceCollection services)
        {
            services.AddDbContext<OttdContext>();
            services.RegisterDependencies<ServersModule>();

            return services;
        }
    }
}
