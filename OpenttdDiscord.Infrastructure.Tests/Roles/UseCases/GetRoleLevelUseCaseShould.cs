using Discord;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.UseCases
{
    public class GetRoleLevelUseCaseShould
    {
        private readonly GetRoleLevelUseCase sut;

        private readonly Fixture fix = new();

        private readonly IAkkaService akkaServiceSubstitute = Substitute.For<IAkkaService>();

        public GetRoleLevelUseCaseShould()
        {
            sut = new(akkaServiceSubstitute);
        }

        [Fact]
        public async Task ReturnUserLevel_ForNonGuildUser()
        {
            IUser nonGuildUser = Substitute.For<IUser>();

            var result = await sut.Execute(nonGuildUser);

            Assert.Equal(
                UserLevel.User,
                result.Right());
        }

        [Fact]
        public async Task ReturnAdminLevel_ForAdministratorUser()
        {
            IGuildUser guildUser = Substitute.For<IGuildUser>();
            guildUser.GuildPermissions.Returns(new GuildPermissions(administrator: true));
            var result = await sut.Execute(guildUser);

            Assert.Equal(
                UserLevel.Admin,
                result.Right());
        }

        [Fact]
        public async Task ReturnProperLevel_FromRoleActor()
        {
            IReadOnlyCollection<ulong> roleIds = fix.Create<ulong[]>();
            IGuildUser guildUser = Substitute.For<IGuildUser>();
            GetRoleLevelResponse response = fix.Create<GetRoleLevelResponse>();
            guildUser.GuildPermissions.Returns(new GuildPermissions(sendMessages: true));
            guildUser.RoleIds.Returns(roleIds);
            guildUser.GuildId.Returns(fix.Create<ulong>());

            akkaServiceSubstitute.SelectAndAsk(
                    MainActors.Paths.Guilds,
                    new GetRoleLevel(
                        guildUser.GuildId,
                        roleIds
                    ))
                .Returns(response);

            var result = await sut.Execute(guildUser);

            Assert.Equal(
                response.RoleLevel,
                result.Right());
        }
    }
}