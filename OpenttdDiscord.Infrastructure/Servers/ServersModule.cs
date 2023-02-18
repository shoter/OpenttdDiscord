using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Modularity;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class ServersModule : IModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IOttdServerRepository, OttdServerRepository>();
            services.AddScoped<IRegisterOttdServerUseCase, RegisterOttdServerUseCase>();
            services.AddScoped<IListOttdServersUseCase, ListOttdServersUseCase>();
            services.AddSingleton<IOttdSlashCommand, RegisterServerCommand>();
            services.AddSingleton<IOttdSlashCommand, ListServersCommand>();
            services.AddScoped<RegisterServerRunner>();
            services.AddScoped<ListServerRunner>();
        }
    }
}
