using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenttdDiscord.Database.Servers
{
    public class ServersModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton(new MySqlConfig()
            {
                ConnectionString = Environment.GetEnvironmentVariable("ottd_discord_connectionstring")
            });
            services.AddSingleton<IServerService, ServerService>();
            services.AddSingleton<IServerRepository, ServerRepository>();
            services.AddSingleton<ISubscribedServerRepository, SubscribedServerRepository>();
            services.AddSingleton<ISubscribedServerService, SubscribedServerService>();
        }
    }
}
