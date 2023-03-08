using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Rcon.Actors;
using OpenttdDiscord.Infrastructure.Rcon.Messages;
using OpenttdDiscord.Infrastructure.Reporting.Actors;
using OpenttdDiscord.Infrastructure.Reporting.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor
    {
        private IListReportChannelsUseCase listReportChannelsUseCase = default!;
        private Dictionary<ulong, IActorRef> reportChannels = new();

        private void ReportConstructor()
        {
            this.listReportChannelsUseCase = SP.GetRequiredService<IListReportChannelsUseCase>();
        }

        private void ReportReady()
        {
            Receive<RegisterReportChannel>(RegisterReportChannel);
            Receive<UnregisterReportChannel>(UnregisterReportChannel);
        }

        private void RegisterReportChannel(RegisterReportChannel msg)
            => CreateNewReportActor(msg.ReportChannel);

        private async Task ReportInit()
        {
            List<ReportChannel> channels = (await listReportChannelsUseCase.Execute(User.Master, server.Id))
                .ThrowIfError()
                .Right();

            foreach (var channel in channels)
            {
                CreateNewReportActor(channel);
            }
        }

        private void UnregisterReportChannel(UnregisterReportChannel msg)
        {
            if (!reportChannels.TryGetValue(msg.channelId, out IActorRef? actor))
            {
                logger.LogWarning($"An attempt was made to remove report channel {msg} but no rcon actor was found");
                return;
            }

            actor.GracefulStop(TimeSpan.FromSeconds(1));
            reportChannels.Remove(msg.channelId);
        }

        private void CreateNewReportActor(ReportChannel channel)
        {
            var reportActor = Context.ActorOf(ReportingActor.Create(SP, channel), $"report-{channel.ChannelId}");
            reportChannels.Add(channel.ChannelId, reportActor);
            logger.LogInformation($"Created report actor for {server.Name} - {channel.ChannelId}");
        }
    }
}
