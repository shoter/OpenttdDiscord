using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class GetRoleRunnerShould : RunnerTestBase
    {
        private readonly GetRoleRunner sut;

        public GetRoleRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.Admin)]
        [InlineData(UserLevel.Moderator)]
        [InlineData(UserLevel.User)]
        public async Task ReturnTextCommandResponse_WithUserLevel_ForGuildUser(UserLevel userLevel)
        {
            await (await WithUserLevel(userLevel)
                    .WithGuildUser()
                    .Run(sut))
                .Received()
                .RespondAsync(
                    Arg.Is<string>(
                        txt => txt.Contains(
                            userLevel.ToString(),
                            StringComparison.InvariantCultureIgnoreCase)),
                    ephemeral: true);
        }

        [Fact]
        public async Task ReturnTextCommandResponse_WithWordUser_ForNonGuildUser()
        {
            await (await WithNonGuildUser()
                    .Run(sut))
                .Received()
                .RespondAsync(
                    Arg.Is<string>(
                        txt => txt.Contains(
                            "user",
                            StringComparison.InvariantCultureIgnoreCase)),
                    ephemeral: true);
        }
    }
}