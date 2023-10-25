using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public abstract class InteractionResponseBase : IInteractionResponse
    {
        public EitherAsyncUnit Execute(IDiscordInteraction interaction) => TryAsync<EitherUnit>(
                async () =>
                {
                    await InternalExecute(interaction);
                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();

        protected abstract Task InternalExecute(IDiscordInteraction interaction);
    }
}
