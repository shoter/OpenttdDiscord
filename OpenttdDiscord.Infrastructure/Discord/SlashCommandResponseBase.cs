using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using static LanguageExt.Prelude;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal abstract class SlashCommandResponseBase : ISlashCommandResponse
    {
        public async Task<EitherUnit> Execute(SocketSlashCommand command)
        {
            return (await TryAsync(async () =>
            {
                await InternalExecute(command);
            })).Match(
                _ =>    Right(Unit.Default),
                fail => Left<IError>(new HumanReadableError("Command execution failed"))
           );
        }

        protected abstract Task InternalExecute(SocketSlashCommand command);
    }
}
