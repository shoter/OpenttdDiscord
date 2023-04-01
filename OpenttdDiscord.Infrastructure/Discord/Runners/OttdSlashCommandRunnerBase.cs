using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.Responses;

namespace OpenttdDiscord.Infrastructure.Discord.Runners
{
    internal abstract class OttdSlashCommandRunnerBase : IOttdSlashCommandRunner
    {
        public EitherAsync<IError, ISlashCommandResponse> Run(SocketSlashCommand command)
        {
            var options = command.Data.Options.ToExtDictionary(o => o.Name, o => o.Value);
            var user = new User(command.User);
            return RunInternal(command, user, options);
        }

        protected abstract EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options);

        protected Either<IError, ulong> CheckIfGuildCommand(SocketSlashCommand command)
        {
            if (command.GuildId.HasValue)
            {
                return command.GuildId!.Value;
            }

            return new HumanReadableError("This command needs to be executed within guild!");
        }

        protected Either<IError, ulong> CheckIfChannelCommand(SocketSlashCommand command)
        {
            if (command.ChannelId.HasValue)
            {
                return command.ChannelId!.Value;
            }

            return new HumanReadableError("This command needs to be executed within channel!");
        }

        protected EitherUnit CheckIfHasCorrectUserLevel(User user, UserLevel level)
        {
            var hasLevel = user.CheckIfHasCorrectUserLevel(level);

            if (!hasLevel)
            {
                return new HumanReadableError("You do not have sufficient privileges to run this use case!");
            }

            return Unit.Default;
        }
    }
}
