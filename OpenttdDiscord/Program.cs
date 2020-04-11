using Discord;
using Discord.WebSocket;
using Discord.Commands;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Openttd;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Commands;
using System.Timers;
using Discord.Rest;
using OpenttdDiscord.Common;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Openttd.Network.Tcp;
using Microsoft.Extensions.Configuration;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Openttd.Network.AdminPort;
using Microsoft.Extensions.Logging;

namespace OpenttdDiscord
{
    internal class Program
    {
        private static DiscordSocketClient client;

        public static async Task Main()
        {
            var config = new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                   .Build();

            DependencyConfig.Init(config);

            using var services = DependencyConfig.ServiceProvider;

            var client = new AdminPortClient(new ServerInfo("192.168.2.100", 3982, "admin_pass"), services.GetRequiredService<IAdminPacketService>(), services.GetRequiredService<ILogger<AdminPortClient>>());

            await client.Join();


            await Task.Delay(-1);
            /*
            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += Log;
            services.GetRequiredService<CommandService>().Log += Log;



            await client.LoginAsync(TokenType.Bot, services.GetRequiredService<OpenttdDiscordConfig>().Token);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            client.Connected += Client_Connected;

            await Task.Delay(-1);*/
        }

        private static async Task Client_Connected()
        {
            await DependencyConfig.ServiceProvider.GetRequiredService<ServerInfoProcessor>().Start();
            await DependencyConfig.ServiceProvider.GetRequiredService<IChatService>().Start();
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
