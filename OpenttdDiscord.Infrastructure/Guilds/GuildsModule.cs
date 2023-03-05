using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
