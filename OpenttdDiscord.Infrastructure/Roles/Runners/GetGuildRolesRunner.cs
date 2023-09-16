using System.Text;
using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class GetGuildRolesRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRolesRepository rolesRepository;
        private readonly DiscordSocketClient discord;

        public GetGuildRolesRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IRolesRepository rolesRepository,
            DiscordSocketClient discord)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.rolesRepository = rolesRepository;
            this.discord = discord;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from _1 in CheckIfHasCorrectUserLevel(
                    user,
                    UserLevel.Moderator).ToAsync()
                from roles in rolesRepository.GetRoles(guildId)
                from response in GenerateResponse(
                    guildId,
                    roles)
                select response;
        }

        private EitherAsync<IError, ISlashCommandResponse> GenerateResponse(
            ulong guildId,
            IEnumerable<GuildRole> guildRoles)
        {
            StringBuilder sbResponse = new();
            var guild = discord.GetGuild(guildId);

            foreach (var role in guildRoles)
            {
                var discordRole = guild.GetRole(role.RoleId);
                sbResponse.AppendLine($"{discordRole.Name} - {role.RoleLevel}");
            }

            return new TextCommandResponse(sbResponse);
        }
    }
}