using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;

namespace OpenttdDiscord.Infrastructure.Discord.CommandRunners
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

        public EitherAsync<IError, IInteractionResponse> Run(ISlashCommandInteraction command)
        {
            var options = new OptionsDictionary(
                command.Data.Options.ToExtDictionary(
                    o => o.Name,
                    o => o.Value));

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

        protected abstract EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options);

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
                return new IncorrectUserLevelError("You do not have sufficient privileges to run this use case!");
            }

            return Unit.Default;
        }
    }
}