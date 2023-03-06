using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Statuses;

namespace OpenttdDiscord.Infrastructure.Ottd.Actions
{
    internal class OttdQueryServerAction : OttdServerAction<QueryServer>
    {
        private readonly ServerStatusEmbedBuilder embedBuilder = new();

        private readonly DiscordSocketClient discord;

        public OttdQueryServerAction(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            : base(serviceProvider, server, client)
        {
            discord = SP.GetRequiredService<DiscordSocketClient>();
        }

        public static Props Create(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            => Props.Create(() => new OttdQueryServerAction(serviceProvider, server, client));

        protected override async Task HandleCommand(QueryServer command)
        {
            logger.LogInformation($"Received command to query status of {server.Name} on {command.ChannelId}");
            ServerStatus serverStatus = await client.QueryServerStatus();
            AdminServerInfo info = serverStatus.AdminServerInfo;

            Embed embed = embedBuilder.CreateServerStatusEmbed(client, serverStatus, info, server.Name);
            IChannel channel = await discord.GetChannelAsync(command.ChannelId);

            if (channel is IMessageChannel msgChannel)
            {
                await msgChannel.SendMessageAsync(embed: embed);
            }
        }
    }
}
