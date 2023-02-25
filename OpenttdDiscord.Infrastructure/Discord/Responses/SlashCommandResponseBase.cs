using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord.Responses
{
    public abstract class SlashCommandResponseBase : ISlashCommandResponse
    {
        public async Task<EitherUnit> Execute(SocketSlashCommand command)
        {
            try
            {
                await InternalExecute(command);
                return Unit.Default;
            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }
        }

        protected abstract Task InternalExecute(SocketSlashCommand command);
    }
}
