using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Modularity
{
    internal interface IModule
    {
        public void RegisterDependencies(IServiceCollection services); 
    }
}
