using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Discord.ModalRunners
{
    public abstract class OttdModalRunnerBase : IOttdModalRunner
    {
        protected OttdModalRunnerBase(IGetRoleLevelUseCase getRoleLevelUseCase)
        {
            GetRoleLevelUseCase = getRoleLevelUseCase;
        }

        private IGetRoleLevelUseCase GetRoleLevelUseCase { get; }

        public EitherAsync<IError, IInteractionResponse> Run(IModalInteraction modalInteraction)
        {
            return
                from userLevel in GetRoleLevelUseCase.Execute(modalInteraction.User)
                from result in RunInternal(
                    modalInteraction,
                    new User(
                        modalInteraction.User,
                        userLevel))
                select result;
        }

        protected abstract EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            User user);
    }
}