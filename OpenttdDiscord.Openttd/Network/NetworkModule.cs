using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    internal class NetworkModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<IOttdClientFactory, OttdClientFactory>();
            new TcpModule().Register(services);
            new UdpModule().Register(services);
        }
    }
}
