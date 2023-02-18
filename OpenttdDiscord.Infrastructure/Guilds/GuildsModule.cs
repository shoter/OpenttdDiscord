using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
