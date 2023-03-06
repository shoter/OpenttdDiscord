using Akka.Actor;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Reporting.Actions;

namespace OpenttdDiscord.Infrastructure.Reporting.Messages
{
    internal record CreateReport(ReportChannel Channel, Player ReportingPlayer, string ReportMessage) : ExecuteServerAction(Channel.ServerId, Channel.ChannelId)
    {
        public override Props CreateCommandActorProps(IServiceProvider serviceProvider, OttdServer server, AdminPortClient client)
            => CreateReportAction.Create(serviceProvider, server, client);
    }
}
