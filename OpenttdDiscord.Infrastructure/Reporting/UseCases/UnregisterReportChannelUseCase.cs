using LanguageExt;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Reporting.Messages;

namespace OpenttdDiscord.Infrastructure.Reporting.UseCases
{
    internal class UnregisterReportChannelUseCase : UseCaseBase, IUnregisterReportChannelUseCase
    {
        private readonly IReportChannelRepository reportChannelRepository;

        public UnregisterReportChannelUseCase(
            IReportChannelRepository reportChannelRepository,
            IAkkaService akkaService)
            : base(akkaService)
        {
            this.reportChannelRepository = reportChannelRepository;
        }

        public EitherAsyncUnit Execute(
            User user,
            Guid serverId,
            ulong guildId,
            ulong channelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from _2 in reportChannelRepository.Delete(
                    serverId,
                    channelId)
                from actor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(
                        new UnregisterReportChannel(
                            serverId,
                            guildId,
                            channelId))
                    .ToAsync()
                select Unit.Default;
        }
    }
}