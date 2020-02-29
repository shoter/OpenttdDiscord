using Discord;
using Discord.WebSocket;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Openttd;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    class Program
    {
        static DiscordSocketClient client;


     
        public static async Task Main()
        {
            var c = DependencyConfig.ServiceProvider.GetService<Config>();

            client = new DiscordSocketClient();
            client.MessageReceived += MessageReceivedAsync;

            await client.LoginAsync(TokenType.Bot, "NDY5NjU1NDE5Mjc5OTAwNjky.XlekaA.4wq4iuyVhL8LQDBR7YvpiQge0y0");
            await client.StartAsync();

            await Task.Delay(-1);
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private static async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }

    }
}
