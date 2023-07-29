using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class GetRoleRunner : OttdSlashCommandRunnerBase
    {
        public GetRoleRunner(IAkkaService akkaService,
                             IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            SocketSlashCommand command,
            User user,
            ExtDictionary<string, object> options)
        {
            if (!(command.User is SocketGuildUser guildUser))
            {
                return new TextCommandResponse("You filthy user!");
            }

            ulong guildId = command.GuildId!.Value;

            return
                 new TextCommandResponse($"Your role level - {user.UserLevel}");
        }
    }
}