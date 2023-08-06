using System.Linq;
using Discord;
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

        public EitherAsync<IError, UserLevel> Execute(IUser user)
        {
            if (!(user is IGuildUser guildUser))
            {
                return UserLevel.User;
            }

            if (guildUser.GuildPermissions.Administrator)
            {
                return UserLevel.Admin;
            }

            ulong guildId = guildUser.GuildId;

            return
                from roleLevelResponse in AkkaService.SelectAndAsk<GetRoleLevelResponse>(
                    MainActors.Paths.Guilds,
                    new GetRoleLevel(
                        guildId,
                        guildUser.RoleIds))
                select roleLevelResponse.RoleLevel;
        }
    }
}