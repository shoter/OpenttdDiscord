using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Modularity
{
    [ExcludeFromCodeCoverage]
    internal interface IModule
    {
        public void RegisterDependencies(IServiceCollection services);
    }
}
