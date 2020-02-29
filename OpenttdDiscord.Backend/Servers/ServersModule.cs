using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenttdDiscord.Backend.Servers
{
    public class ServersModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddScoped<IServerRepository, ServerRepository>();
            services.AddScoped<ISubscribedServerRepository, SubscribedServerRepository>();
            services.AddScoped<ISubscribedServerService, SubscribedServerService>();
        }
    }
}
