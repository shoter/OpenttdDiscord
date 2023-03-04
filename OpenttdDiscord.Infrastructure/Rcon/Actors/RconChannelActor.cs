using Akka.Actor;
using Discord;
using Discord.WebSocket;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Rcon.Actors
{
    internal class RconChannelActor : ReceiveActorBase
    {
        private readonly RconChannel channel;

        private readonly OttdServer server;

        private readonly IAdminPortClient client;

        private readonly IMessageChannel messageChannel;

        public RconChannelActor(
            IServiceProvider sp,
            RconChannel channel,
            OttdServer server,
            IAdminPortClient client,
            DiscordSocketClient discord)
            : base(sp)
        {
            this.channel = channel;
            this.server = server;
            this.client = client;
            this.messageChannel = (IMessageChannel)discord.GetChannel(channel.ChannelId);

            Ready();
        }

        public static Props Create(IServiceProvider sp, RconChannel channel, OttdServer server, IAdminPortClient client, DiscordSocketClient discord)
            => Props.Create(() => new RconChannelActor(sp, channel, server, client, discord));

        private void Ready()
        {

        }
    }
}
