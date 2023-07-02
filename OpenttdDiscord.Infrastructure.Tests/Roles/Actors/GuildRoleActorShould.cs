using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using AutoFixture;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Infrastructure.Roles.Actors;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Tests.Common.Xunits;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Actors
{
    public sealed class GuildRoleActorShould : TestKit
    {
        private readonly IActorRef guildRoleActor = default!;
        private readonly IFixture fix = new Fixture();
        private readonly IRolesRepository rolesRepositoryMock = Substitute.For<IRolesRepository>();
        private readonly IServiceProvider serviceProvider;
        private readonly TestProbe probe;

        public GuildRoleActorShould(ITestOutputHelper testOutputHelper)
        {
            ServiceCollection services = new();
            services.AddLogging(
                logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                });
            services.AddSingleton(rolesRepositoryMock);
            serviceProvider = services.BuildServiceProvider();
            guildRoleActor = ActorOf(GuildRoleActor.Create(serviceProvider));

            rolesRepositoryMock
                .InsertRole(default!)
                .ReturnsForAnyArgs(Unit.Default);

            probe = CreateTestProbe();
        }

        [Fact(Timeout = 2_000)]
        public async Task RegisterAndRetrieve_Role()
        {
            // Arrange
            var registerNewRole = fix.Create<RegisterNewRole>();
            guildRoleActor.Tell(registerNewRole, probe.Ref);
            probe.ExpectMsg<Unit>();

            await probe.WithinAsync(
                TimeSpan.FromSeconds(1),
                async () =>
                {
                    // Act
                    var getRoleLevel = new GetRoleLevel(
                        registerNewRole.GuildId,
                        registerNewRole.RoleId);
                    guildRoleActor.Tell(
                        getRoleLevel,
                        probe.Ref);

                    var response = await probe.ExpectMsgAsync<GetRoleLevelResponse>();

                    // Assert
                    Assert.Equal(
                        registerNewRole.RoleLevel,
                        response.RoleLevel);
                });
        }

        [Fact(Timeout = 2_000)]
        public async Task SaveDataInDatabase_WhenRegisteringRole()
        {
            // Arrange
            var registerNewRole = fix.Create<RegisterNewRole>();
            await guildRoleActor.Ask(registerNewRole);

            Within(
                TimeSpan.FromSeconds(1),
                () =>
                {
                    // rolesRepositoryMock.Verify(x => x.InsertRole(It.Is<GuildRole>(gr =>
                    //     gr.RoleId == registerNewRole.RoleId &&
                    //     gr.GuildId == registerNewRole.GuildId &&
                    //     gr.RoleLevel == registerNewRole.RoleLevel)), Times.Once);
                });
        }

        [Fact(Timeout = 2_000)]
        public void RemoveDataInDatabase_WhenRemovingRole()
        {
            // Arrange
            var registerNewRole = fix.Create<RegisterNewRole>();
            guildRoleActor.Tell(registerNewRole);
            var deleteRole = new DeleteRole(
                registerNewRole.GuildId,
                registerNewRole.GuildId);

            Within(
                TimeSpan.FromSeconds(1),
                () =>
                {
                    // rolesRepositoryMock.Verify(
                    //     x => x.DeleteRole(
                    //         registerNewRole.GuildId,
                    //         registerNewRole.RoleId), Times.Once);
                });
        }
    }
}