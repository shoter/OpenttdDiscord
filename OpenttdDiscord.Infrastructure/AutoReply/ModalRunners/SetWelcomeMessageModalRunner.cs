using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.AutoReply.ModalRunners
{
    internal class SetWelcomeMessageModalRunner : OttdModalRunnerBase
    {
        public SetWelcomeMessageModalRunner(IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            Dictionary<string, IComponentInteractionData> components,
            User user)
        {
            return new TextResponse("Dupa XD");
        }
    }
}