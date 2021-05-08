﻿using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    internal class UdpModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<IUdpPacketService, UdpPacketService>();
            services.AddSingleton<IUdpOttdClientFactory, UdpOttdClientFactory>();
        }
    }
}
