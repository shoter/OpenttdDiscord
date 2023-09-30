using Discord;
using Discord.WebSocket;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class RegisterBotRoleRunnerShould : RunnerTestBase
    {
        private readonly RegisterBotRoleRunner sut;

        public RegisterBotRoleRunnerShould()
        {
            sut = new(
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            IRole roleSub = Substitute.For<IRole>();
            var result = await WithGuildUser()
                .WithOption("role", roleSub, ApplicationCommandOptionType.Role)
                .WithOption("role-level", (long)UserLevel.User)
                .WithUserLevel(userLevel)
                .RunExt(sut);

            Assert.True(result.IsLeft);
            Assert.True(result.Left() is IncorrectUserLevelError);
        }
    }
}
