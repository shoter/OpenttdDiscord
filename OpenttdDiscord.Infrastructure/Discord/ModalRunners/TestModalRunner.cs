using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.ModalResponses;

namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public class TestModalRunner : OttdModalRunnerBase
    {
        public TestModalRunner(IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, IModalResponse> RunInternal(
            IModalInteraction modal,
            User user)
        {
        }
    }
}