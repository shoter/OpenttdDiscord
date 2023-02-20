using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal abstract class OttdSlashCommandRunnerBase : IOttdSlashCommandRunner
    {
        public Task<Either<IError, ISlashCommandResponse>> Run(SocketSlashCommand command)
        {
            var options = command.Data.Options.ToExtDictionary(o => o.Name, o => o.Value);
            return RunInternal(command, options);
        }

        protected abstract Task<Either<IError, ISlashCommandResponse>> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options);
    }
}
