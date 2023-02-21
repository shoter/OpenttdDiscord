﻿using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal class OttdQueryServerAction : OttdServerAction<QueryServer>
    {
        DiscordSocketClient discord;
        public OttdQueryServerAction(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            : base(serviceProvider, server, client)
        {
            discord = SP.GetRequiredService<DiscordSocketClient>();
        }

        public static Props Create(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            => Props.Create(() => new OttdQueryServerAction(serviceProvider, server, client));

        protected override async Task HandleCommand(QueryServer command)
        {
            this.logger.LogInformation($"Received command to query status of {server.Name} on {command.ChannelId}");
            ServerStatus serverStatus = await client.QueryServerStatus();
            AdminServerInfo info = serverStatus.AdminServerInfo;

            Embed embed = CreateServerStatusEmbed(serverStatus, info);
            IChannel channel = await discord.GetChannelAsync(command.ChannelId);

            if (channel is IMessageChannel msgChannel)
            {
                await msgChannel.SendMessageAsync(embed: embed);
            }
        }

        private Embed CreateServerStatusEmbed(ServerStatus serverStatus, AdminServerInfo info)
        {
            EmbedBuilder embedBuilder = new();
            embedBuilder.WithTitle($"{server.Name} Status");

            embedBuilder.AddField("Players", serverStatus.Players.Count);
            embedBuilder.AddField("Map Size", $"{info.MapWidth}x{info.MapHeight}", false);
            embedBuilder.AddField("Year", info.Date, false);

            embedBuilder.AddField("Map Name", info.MapName);
            embedBuilder.AddField("Climate", info.Landscape, false);
            embedBuilder.AddField("Server address", $"{client.ServerInfo.ServerIp}:{client.ServerInfo.ServerPort}", false);

            var embed = embedBuilder.Build();
            return embed;
        }

        protected override void PostStop()
        {
            base.PostStop();
        }
    }
}