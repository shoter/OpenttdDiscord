using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class TcpModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddTransient<ITcpOttdClient, TcpOttdClient>();
            services.AddSingleton<ITcpPacketReader, TcpPacketReader>();
            services.AddSingleton<ITcpPacketCreator, TcpPacketCreator>();
        }
    }
}
