using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
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

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            return from _ in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                select new ModalResponse(testModal) as IInteractionResponse;
        }
    }
}