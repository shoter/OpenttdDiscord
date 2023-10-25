using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actions
{
    internal class QueryDebugInfoAction : OttdServerAction<QueryDebugInfo>
    {
        private readonly DiscordSocketClient discord;

        public QueryDebugInfoAction(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            : base(serviceProvider, server, client)
        {
            this.discord = SP.GetRequiredService<DiscordSocketClient>();
        }

        internal static Props Create(
            IServiceProvider sp,
            OttdServer server,
            IAdminPortClient client)
            => Props.Create(() => new QueryDebugInfoAction(sp, server, client));

        protected override async Task HandleCommand(QueryDebugInfo command)
        {
            using MemoryStream ms = new();
            using (StreamWriter sw = new(ms, leaveOpen: true))
            {
                ServerStatus status = await client.QueryServerStatus();
                IMainData data = await client.GetMainData();

                var debugInfo = new
                {
                    status,
                    data,
                };

                sw.WriteLine(JsonConvert.SerializeObject(debugInfo, Formatting.Indented));
            }

            IChannel channel = await discord.GetChannelAsync(command.ChannelId);

            if (channel is IMessageChannel msgChannel)
            {
                await msgChannel.SendFileAsync(ms, $"{server.Name}.debug.txt", $"{server.Name} debug status");
            }
        }
    }
}
