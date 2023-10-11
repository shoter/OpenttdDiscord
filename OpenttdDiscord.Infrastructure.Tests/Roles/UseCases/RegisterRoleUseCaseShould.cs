using Discord;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.UseCases
{
    public class RegisterRoleUseCaseShould
    {
        private readonly RegisterRoleUseCase sut;

        private readonly Fixture fix = new();

        private readonly IAkkaService akkaServiceSubstitute = Substitute.For<IAkkaService>();

        public RegisterRoleUseCaseShould()
        {
            sut = new(akkaServiceSubstitute);
        }

        [Fact]
        public async Task SendMessageAboutNewRole_ToActor()
        {
            ulong guildId = fix.Create<ulong>();
            IRole role = Substitute.For<IRole>();
            role.Id.Returns(fix.Create<ulong>());
            UserLevel userLevel = UserLevel.Moderator;

            akkaServiceSubstitute.SelectAndAsk<object>(
                    MainActors.Paths.Guilds,
                    Arg.Is<UpsertRole>(_ => true))
                .Returns(Unit.Default);

            await sut.Execute(
                guildId,
                role,
                userLevel);

            await akkaServiceSubstitute
                .Received(1)
                .SelectAndAsk<object>(
                    MainActors.Paths.Guilds,
                    Arg.Is<UpsertRole>(
                        msg =>
                            msg.GuildId == guildId &&
                            msg.RoleLevel == UserLevel.Moderator &&
                            msg.RoleId == role.Id));
        }

        [Fact]
        public async Task ThrowError_WhenRoleLevelIsIncorrect()
        {
            ulong guildId = fix.Create<ulong>();
            IRole role = Substitute.For<IRole>();
            role.Id.Returns(fix.Create<ulong>());
            var userLevel = (UserLevel) 123456;

            var result = await sut.Execute(
                guildId,
                role,
                userLevel);

            Assert.True(result.IsLeft);
        }
    }
}