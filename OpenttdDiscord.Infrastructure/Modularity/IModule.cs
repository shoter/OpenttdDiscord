using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Modularity
{
    internal interface IModule
    {
        [ExcludeFromCodeCoverage]
        public void RegisterDependencies(IServiceCollection services);
    }
}
