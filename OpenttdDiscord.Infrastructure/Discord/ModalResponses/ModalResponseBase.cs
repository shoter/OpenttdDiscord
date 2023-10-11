using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord.ModalResponses
{
    public abstract class ModalResponseBase : IModalResponse
    {
        public EitherAsyncUnit Execute(IModalInteraction modal) => TryAsync<EitherUnit>(
                async () =>
                {
                    await InternalExecute(modal);
                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();

        protected abstract Task InternalExecute(IModalInteraction command);
    }
}