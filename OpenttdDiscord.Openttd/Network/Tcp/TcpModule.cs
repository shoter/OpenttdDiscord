using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    internal class TcpModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddTransient<ITcpOttdClient, TcpOttdClient>();
            services.AddSingleton<ITcpPacketService, TcpPacketService>();
        }
    }
}
