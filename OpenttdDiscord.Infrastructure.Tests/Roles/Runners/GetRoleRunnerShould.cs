using AutoFixture;
using Discord;
using NSubstitute;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using Array = System.Array;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class GetRoleRunnerShould : RunnerTestBase
    {
        private readonly GetRoleRunner sut;

        public GetRoleRunnerShould()
        {
            sut = new(
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.Admin)]
        [InlineData(UserLevel.Moderator)]
        [InlineData(UserLevel.User)]
        public async Task ReturnTextCommandResponse_WithWordUser_ForNonGuildUser(UserLevel userLevel)
        {
            await (await WithUserLevel(userLevel)
                    .WithGuildUser()
                    .Run(sut))
                .Received()
                .RespondAsync(Arg.Is<string>(txt => txt.Contains(userLevel.ToString(), StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}