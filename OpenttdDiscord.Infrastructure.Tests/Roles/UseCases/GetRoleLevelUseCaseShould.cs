using Discord;
using Discord.WebSocket;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.UseCases
{
    public class GetRoleLevelUseCaseShould
    {
        private readonly GetRoleLevelUseCase sut;

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
    }
}
