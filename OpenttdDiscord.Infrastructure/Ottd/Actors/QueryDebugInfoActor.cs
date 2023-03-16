using Akka.Actor;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal class QueryDebugInfoActor : OttdServerAction<QueryDebugInfo>
    {
        private readonly ulong channelId;
        private readonly DiscordSocketClient discord;

        public QueryDebugInfoActor(
            IServiceProvider serviceProvider,
            OttdServer server,
            IAdminPortClient client,
            ulong channelId)
            : base(serviceProvider, server, client)
        {
            this.channelId = channelId;
            this.discord = SP.GetRequiredService<DiscordSocketClient>();
        }

        public static Props Create(
            IServiceProvider sp,
            OttdServer server,
            IAdminPortClient client,
            ulong channelId) => Props.Create(() => new QueryDebugInfoActor(sp, server, client, channelId));

        protected override Task HandleCommand(QueryDebugInfo command)
        {
            var status = 
        }
    }
}
