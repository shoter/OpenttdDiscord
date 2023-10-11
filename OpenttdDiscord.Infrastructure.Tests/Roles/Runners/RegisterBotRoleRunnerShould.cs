using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using OpenttdDiscord.Infrastructure.Tests.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class RegisterBotRoleRunnerShould : RunnerTestBase
    {
        private readonly RegisterRoleRunner sut;

        private readonly IRegisterRoleUseCase registerRoleUseCaseSub = Substitute.For<IRegisterRoleUseCase>();;

        public RegisterBotRoleRunnerShould()
        {
            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                registerRoleUseCaseSub);
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

        [Fact]
        public async Task ExecuteUseCase()
        {
            IRole roleSub = Substitute.For<IRole>();
            await
                WithOption(
                        "role",
                        roleSub,
                        ApplicationCommandOptionType.Role)
                    .WithOption(
                        "role-level",
                        (long) UserLevel.Admin)
                    .RunExt(sut);

            registerRoleUseCaseSub
                .Received()
                .Execute(
                    GuildId,
                    roleSub,
                    UserLevel.Admin);
        }
    }
}