using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Testing.ModalRunners
{
    public class TestModalRunner : OttdModalRunnerBase
    {
        public TestModalRunner(IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            Dictionary<string, IComponentInteractionData> components,
            User user)
        {
            var component = components["labelId"];
            string value = component.Value;
            return new TextResponse("You typed " + value, ephemeral: false);
        }
    }
}