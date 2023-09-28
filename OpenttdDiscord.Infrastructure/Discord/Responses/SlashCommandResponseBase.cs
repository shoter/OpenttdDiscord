using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
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
