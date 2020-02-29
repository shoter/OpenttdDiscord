using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Commands;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    public static class DependencyConfig
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        static DependencyConfig()
        {
            var services = new ServiceCollection();

            new ConfigModule().Register(services);
            new ServersModule().Register(services);
            new CommandsModule().Register(services);
            new UdpModule().Register(services);

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
