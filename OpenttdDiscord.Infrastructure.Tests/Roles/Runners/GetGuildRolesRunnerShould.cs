using Discord;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class GetGuildRolesRunnerShould : CommandRunnerTestBase
    {
        private readonly IRolesRepository rolesRepositorySub = Substitute.For<IRolesRepository>();

        private readonly IDiscordClient discordClientSub = Substitute.For<IDiscordClient>();

        private readonly GetGuildRolesRunner sut;

        public GetGuildRolesRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                rolesRepositorySub,
                discordClientSub
            );
        }

        [Fact]
        public async Task Return_ListOfRoles_WithCorrectUserLevel()
        {
            var guildRoles = fix.Create<List<GuildRole>>();

            rolesRepositorySub
                .GetRoles(GuildId)
                .Returns(guildRoles);

            IGuild guildSub = Substitute.For<IGuild>();
            discordClientSub.GetGuildAsync(GuildId)
                .Returns(guildSub);

            foreach (var guildRole in guildRoles)
            {
                var roleSub = Substitute.For<IRole>();
                roleSub.Name.Returns($"id-{guildRole.RoleId}");
                guildSub.GetRole(guildRole.RoleId)
                    .Returns(roleSub);
            }

            await (await WithGuildUser()
                    .WithUserLevel(UserLevel.Moderator)
                    .Run(sut))
                .Received()
                .RespondAsync(
                    Arg.Is<string>(
                        txt =>
                            guildRoles.All(
                                guildRole =>
                                    txt
                                        .Split(
                                            Environment.NewLine,
                                            StringSplitOptions.None)
                                        .Count(
                                            line => line.Contains(
                                                        guildRole.RoleLevel.ToString(),
                                                        StringComparison.InvariantCultureIgnoreCase) &&
                                                    line.Contains(
                                                        $"id-{guildRole.RoleId}",
                                                        StringComparison.InvariantCultureIgnoreCase)) == 1)),
                    ephemeral: true);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        public async Task NotExecuteForNonModerator(UserLevel userLevel)
        {
            await WithGuildUser()
                .WithUserLevel(userLevel)
                .NotExecuteFor(
                    sut,
                    userLevel);
        }
    }
}