using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Domain.Guilds.UseCases;
using OpenttdDiscord.Infrastructure.Guilds.UseCases;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    internal class GuildsModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IGetAllGuildsUseCase, GetAllGuildsUseCase>();
        }
    }
}
