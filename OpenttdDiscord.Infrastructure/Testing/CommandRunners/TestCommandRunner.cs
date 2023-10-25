using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Testing.Modals;

namespace OpenttdDiscord.Infrastructure.Testing.CommandRunners
{
    internal class TestCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly TestModal testModal;

        public TestCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            TestModal testModal)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.testModal = testModal;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            return from _ in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                select new ModalCommandResponse(testModal) as ISlashCommandResponse;
        }
    }
}