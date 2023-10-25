using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    public abstract class SlashCommandResponseBase : ISlashCommandResponse
    {
        public EitherAsyncUnit Execute(ISlashCommandInteraction command) => TryAsync<EitherUnit>(
                async () =>
                {
                    await InternalExecute(command);
                    return Unit.Default;
                })
            .ToEitherAsyncErrorFlat();

        protected abstract Task InternalExecute(ISlashCommandInteraction command);
    }
}
