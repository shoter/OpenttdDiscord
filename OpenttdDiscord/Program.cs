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
using OpenttdDiscord.Database.Servers;
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
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Admins;

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

            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += Log;
            services.GetRequiredService<CommandService>().Log += Log;

            await client.LoginAsync(TokenType.Bot, services.GetRequiredService<OpenttdDiscordConfig>().Token);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            client.Connected += Client_Connected;

            await Task.Delay(-1);
        }

        private static async Task Client_Connected()
        {
            await DependencyConfig.ServiceProvider.GetRequiredService<ServerInfoProcessor>().Start();
            await DependencyConfig.ServiceProvider.GetRequiredService<IChatService>().Start();
            await DependencyConfig.ServiceProvider.GetRequiredService<IAdminService>().Start();
            DependencyConfig.ServiceProvider.GetRequiredService<IServerService>().NewServerPasswordRequestAdded += Program_NewServerPasswordRequestAdded; ;
        }

        private static void Program_NewServerPasswordRequestAdded(object sender, NewServerPassword e)
        {
            client.GetUser(e.UserId).SendMessageAsync($"To complete registration of server please provide password to server {e.ServerName} in next message to this bot.");
        }

         private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
