using AutoFixture;
using Discord;
using NSubstitute;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class GetGuildRolesRunnerShould
    {
        private readonly IAkkaService akkaServiceSub = Substitute.For<IAkkaService>();

        private readonly IGetRoleLevelUseCase getRoleLevelUseCaseSub = Substitute.For<IGetRoleLevelUseCase>();

        private readonly IRolesRepository rolesRepositorySub = Substitute.For<IRolesRepository>();

        private readonly IDiscordClient discordClientSub = Substitute.For<IDiscordClient>();

        private readonly GetGuildRolesRunner sut;

        private readonly Fixture fix = new();

        public GetGuildRolesRunnerShould()
        {
            sut = new(
                akkaServiceSub,
                getRoleLevelUseCaseSub,
                rolesRepositorySub,
                discordClientSub
                );

            getRoleLevelUseCaseSub.Execute(default!)
                .ReturnsForAnyArgs(UserLevel.Admin);
        }

        [Fact]
        public Task ReturnTextCommandResponse_WithWordUser_ForNonGuildUser()
        {
            return Task.CompletedTask;
        }
    }
}
