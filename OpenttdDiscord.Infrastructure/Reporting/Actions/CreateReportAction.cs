using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Actions;
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
        }

        public static Props Create(IServiceProvider serviceProvider, OttdServer server, IAdminPortClient client)
            => Props.Create(() => new CreateReportAction(serviceProvider, server, client));

        protected override async Task HandleCommand(CreateReport command)
        {
            var channel = (IMessageChannel)await discord.GetChannelAsync(command.Channel.ChannelId);

            using MemoryStream ms = new();
            using (StreamWriter sw = new(ms, leaveOpen: true))
            {
                sw.WriteLine("Omae-ha shinde iru!");
                sw.WriteLine("Nani!?");
            }

            ms.Position = 0;
            await channel.SendFileAsync(ms,
                $"Report-{command.ReportingPlayer.Name}-{DateTime.Now:yyyy_MM_dd_HH_mm}.report.txt",
                text: $"{command.ReportingPlayer.Name}({command.ReportingPlayer.Hostname}) have reported an issue: \n{Format.BlockQuote(command.ReportMessage)}");
        }
    }
}
