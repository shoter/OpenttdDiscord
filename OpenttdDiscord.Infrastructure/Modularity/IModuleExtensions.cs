using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Modularity
{
    internal static class IModuleExtensions
    {
        public static IServiceCollection RegisterDependencies<TModule>(this IServiceCollection services)
            where TModule : IModule, new()
        {
            var module = new TModule();
            module.RegisterDependencies(services);
            return services;
        }
    }
}
