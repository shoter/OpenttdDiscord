using Discord;
using Discord.WebSocket;
using Discord.Commands;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Openttd;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using OpenttdDiscord.Backend.Servers;
using OpenttdDiscord.Commands;

namespace OpenttdDiscord
{
    class Program
    {
        static DiscordSocketClient client;


     
        public static async Task Main()
        {
            using (var services = DependencyConfig.ServiceProvider)
            {
                client = services.GetRequiredService<DiscordSocketClient>();
                client.Log += Log;
                services.GetRequiredService<CommandService>().Log += Log;


                await client.LoginAsync(TokenType.Bot, services.GetRequiredService<OpenttdDiscordConfig>().Token);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
      
    }
}
