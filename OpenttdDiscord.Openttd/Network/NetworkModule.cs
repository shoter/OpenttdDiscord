using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Network.Udp;

namespace OpenttdDiscord.Openttd.Network
{
    internal class NetworkModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            new UdpModule().Register(services);
        }
    }
}
