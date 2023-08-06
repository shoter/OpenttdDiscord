using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;

namespace OpenttdDiscord.Infrastructure.Discord.Runners
{
    internal abstract class OttdSlashCommandRunnerBase : IOttdSlashCommandRunner
    {
        private IGetRoleLevelUseCase GetRoleLevelUseCase { get; }

        protected OttdSlashCommandRunnerBase(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
        {
            AkkaService = akkaService;
            GetRoleLevelUseCase = getRoleLevelUseCase;
        }

        protected IAkkaService AkkaService { get; }

        public EitherAsync<IError, ISlashCommandResponse> Run(ISlashCommandInteraction command)
        {
            var options = command.Data.Options.ToExtDictionary(
                o => o.Name,
                o => o.Value);

            return
                from userLevel in GetRoleLevelUseCase.Execute(command.User)
                from result in RunInternal(
                    command,
                    new User(
                        command.User,
                        userLevel),
                    options)
                select result;
        }

        protected abstract EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options);

        protected Either<IError, ulong> EnsureItIsGuildCommand(ISlashCommandInteraction command)
        {
            if (command.GuildId.HasValue)
            {
                return command.GuildId!.Value;
            }

            return new HumanReadableError("This command needs to be executed within guild!");
        }

        protected Either<IError, ulong> EnsureItIsChannelCommand(ISlashCommandInteraction command)
        {
            if (command.ChannelId.HasValue)
            {
                return command.ChannelId!.Value;
            }

            return new HumanReadableError("This command needs to be executed within channel!");
        }

        protected EitherUnit CheckIfHasCorrectUserLevel(
            User user,
            UserLevel level)
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