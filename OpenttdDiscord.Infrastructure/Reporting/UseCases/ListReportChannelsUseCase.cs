using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Reporting.UseCases
{
    internal class ListReportChannelsUseCase : UseCaseBase, IListReportChannelsUseCase
    {
        private readonly IReportChannelRepository reportChannelRepository;

        public ListReportChannelsUseCase(IReportChannelRepository reportChannelRepository)
        {
            this.reportChannelRepository = reportChannelRepository;
        }

        public EitherAsync<IError, List<ReportChannel>> Execute(User user, Guid serverId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from rconChannels in reportChannelRepository.GetReportChannelsForServer(serverId)
                select rconChannels;
        }

        public EitherAsync<IError, List<ReportChannel>> Execute(User user, ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from rconChannels in reportChannelRepository.GetReportChannelsForGuild(guildId)
                select rconChannels;
        }
    }
}
