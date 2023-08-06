using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Reporting.Messages;

namespace OpenttdDiscord.Infrastructure.Reporting.UseCases
{
    internal class RegisterReportChannelUseCase : UseCaseBase, IRegisterReportChannelUseCase
    {
        private readonly IReportChannelRepository reportChannelRepository;

        private readonly IAkkaService akkaService;

        public RegisterReportChannelUseCase(
            IReportChannelRepository reportChannelRepository,
            IAkkaService akkaService)
        {
            this.reportChannelRepository = reportChannelRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, ReportChannel reportChannel)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in reportChannelRepository.Insert(reportChannel)
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(new RegisterReportChannel(reportChannel)).ToAsync()
                select _3;
        }
    }
}
