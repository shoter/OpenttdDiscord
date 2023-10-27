using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.EventLogs.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Actions;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Reporting.Messages;

namespace OpenttdDiscord.Infrastructure.Reporting.Actions
{
    internal class CreateReportAction : OttdServerAction<CreateReport>
    {
        private readonly DiscordSocketClient discord;
        public CreateReportAction(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            : base(serviceProvider, server, client)
        {
            this.discord = serviceProvider.GetRequiredService<DiscordSocketClient>();
            parent.Tell(new SubscribeToAdminEvents(Self));
        }

        protected override void Ready()
        {
            base.Ready();
            ReceiveIgnore<IAdminEvent>();
        }

        public static Props Create(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            => Props.Create(() => new CreateReportAction(serviceProvider, server, client));

        protected override async Task HandleCommand(CreateReport command)
        {
            var channel = (IMessageChannel)await discord.GetChannelAsync(command.Channel.ChannelId);
            var serverStatus = await client.QueryServerStatus();

            using MemoryStream ms = new();
            using (StreamWriter sw = new(ms, leaveOpen: true))
            {
                WritePlayerSection(serverStatus, sw);
                await WriteEventSection(sw);
            }

            ms.Position = 0;
            await channel.SendFileAsync(ms,
                $"Report-{server.Name}-{command.ReportingPlayer.Name}-{DateTime.Now:yyyy_MM_dd_HH_mm}.report.txt",
                text: $"{command.ReportingPlayer.Name}({command.ReportingPlayer.Hostname}) have reported an issue on {server.Name}: \n{Format.BlockQuote(command.ReportMessage)}");

            string message = $"Report has been sent";
            client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, default, message));
        }

        private async Task WriteEventSection(StreamWriter sw)
        {
            WriteSectionStart(sw, "Event log");
            var retrieved = await parent.Ask<RetrievedEventLog>(new RetrieveEventLog(server.Id, server.GuildId));
            foreach (var line in retrieved.Messages.Reverse())
            {
                sw.WriteLine(line);
            }

            WriteSectionEnd(sw);
        }

        private void WritePlayerSection(OpenTTDAdminPort.Game.ServerStatus serverStatus, StreamWriter sw)
        {
            WriteSectionStart(sw, "Players");
            foreach (var p in serverStatus.Players.Values)
            {
                sw.WriteLine($"{p.ClientId}. {p.Name}({p.Hostname}) - {p.PlayingAs}");
            }

            WriteSectionEnd(sw);
        }

        private void WriteSectionStart(StreamWriter sw, string sectionName)
        {
            sw.Write("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= ");
            sw.WriteLine(sectionName);
        }

        private void WriteSectionEnd(StreamWriter sw)
        {
            sw.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            sw.WriteLine();
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
        }
    }
}
