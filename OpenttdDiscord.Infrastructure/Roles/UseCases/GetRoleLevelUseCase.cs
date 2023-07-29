using System.Linq;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.UseCases
{
    public class GetRoleLevelUseCase : IGetRoleLevelUseCase
    {
        private IAkkaService AkkaService { get; }

        public GetRoleLevelUseCase(IAkkaService akkaService)
        {
            this.AkkaService = akkaService;
        }

        public EitherAsync<IError, UserLevel> Execute(SocketUser user)
        {
            if (!(user is SocketGuildUser guildUser))
            {
                return UserLevel.User;
            }

            if (guildUser.GuildPermissions.Administrator)
            {
                return UserLevel.Admin;
            }

            ulong guildId = guildUser.Guild.Id;

            return
                from guildsActor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from roleLevelResponse in guildsActor.TryAsk<GetRoleLevelResponse>(
                    new GetRoleLevel(
                        guildId,
                        guildUser.Roles.Select(x => x.Id)))
                select roleLevelResponse.RoleLevel;
        }
    }
}