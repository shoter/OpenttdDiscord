using System.Text;
using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class GetGuildRolesRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRolesRepository rolesRepository;
        private readonly IDiscordClient discord;

        public GetGuildRolesRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IRolesRepository rolesRepository,
            IDiscordClient discord)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.rolesRepository = rolesRepository;
            this.discord = discord;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
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

        private EitherAsync<IError, IInteractionResponse> GenerateResponse(
            ulong guildId,
            IEnumerable<GuildRole> guildRoles) => TryAsync<Either<IError, IInteractionResponse>>(
                async () =>
                {
                    StringBuilder sbResponse = new();
                    var guild = await discord.GetGuildAsync(guildId);

                    foreach (var role in guildRoles)
                    {
                        var discordRole = guild.GetRole(role.RoleId);
                        sbResponse.AppendLine($"{discordRole.Name} - {role.RoleLevel}");
                    }

                    return new TextResponse(sbResponse);
                })
            .ToEitherAsyncErrorFlat();
    }
}