using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using OpenttdDiscord.Infrastructure.Tests.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class RegisterBotRoleRunnerShould : RunnerTestBase
    {
        private readonly RegisterBotRoleRunner sut;

        public RegisterBotRoleRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            IRole roleSub = Substitute.For<IRole>();
            await
                WithOption(
                        "role",
                        roleSub,
                        ApplicationCommandOptionType.Role)
                    .WithOption(
                        "role-level",
                        (long) UserLevel.User)
                    .NotExecuteFor(
                        sut,
                        userLevel);
        }
    }
}