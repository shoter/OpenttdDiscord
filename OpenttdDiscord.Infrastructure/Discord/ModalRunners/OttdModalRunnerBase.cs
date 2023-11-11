using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.Errors;
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
                    GetInteractionComponentDictionary(modalInteraction),
                    new User(
                        modalInteraction.User,
                        userLevel))
                select result;
        }

        protected Either<IError, ulong> EnsureItIsGuildModal(IModalInteraction modal)
        {
            if (modal.GuildId.HasValue)
            {
                return modal.GuildId!.Value;
            }

            return new HumanReadableError("This command needs to be executed within guild!");
        }

        private Dictionary<string, IComponentInteractionData> GetInteractionComponentDictionary(
            IModalInteraction interaction) => interaction.Data.Components.ToDictionary(x => x.CustomId);

        protected abstract EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            Dictionary<string, IComponentInteractionData> components,
            User user);

        protected EitherUnit CheckIfHasCorrectUserLevel(
            User user,
            UserLevel level)
        {
            var hasLevel = user.CheckIfHasCorrectUserLevel(level);

            if (!hasLevel)
            {
                return new IncorrectUserLevelError("You do not have sufficient privileges to run this use case!");
            }

            return Unit.Default;
        }
    }
}