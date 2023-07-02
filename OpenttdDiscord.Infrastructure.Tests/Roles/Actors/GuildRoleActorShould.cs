using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Akkas;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Actors;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Tests.Common.Xunits;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Actors
{
    public sealed class GuildRoleActorShould : TestKit
    {
        private readonly IFixture fix = new Fixture();
        private readonly IActorRef guildRoleActor = default!;
        private readonly TestProbe probe;
        private readonly IRolesRepository rolesRepositoryMock = Substitute.For<IRolesRepository>();
        private readonly IServiceProvider serviceProvider;

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

            rolesRepositoryMock
                .DeleteRole(
                    default!,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            rolesRepositoryMock
                .DeleteRole(default!)
                .ReturnsForAnyArgs(Unit.Default);

            probe = CreateTestProbe();
        }

        [Fact(Timeout = 2_000)]
        public async Task RegisterAndRetrieve_Role()
        {
            // Arrange
            RegisterNewRole? registerNewRole = fix.Create<RegisterNewRole>();
            guildRoleActor.Tell(
                registerNewRole,
                probe.Ref);
            probe.ExpectMsg<Unit>();

            await probe.WithinAsync(
                TimeSpan.FromSeconds(1),
                async () =>
                {
                    // Act
                    GetRoleLevel getRoleLevel = new(
                        registerNewRole.GuildId,
                        registerNewRole.RoleId);
                    guildRoleActor.Tell(
                        getRoleLevel,
                        probe.Ref);

                    GetRoleLevelResponse? response = await probe.ExpectMsgAsync<GetRoleLevelResponse>();

                    // Assert
                    Assert.Equal(
                        registerNewRole.RoleLevel,
                        response.RoleLevel);
                });
        }

        [Fact(Timeout = 2_000)]
        public async Task NotBeAbleTo_RegisterSameRole_Twice()
        {
            // Arrange
            RegisterNewRole? registerNewRole = fix.Create<RegisterNewRole>();
            guildRoleActor.Tell(
                registerNewRole,
                probe.Ref);
            probe.ExpectMsg<Unit>();

            await probe.WithinAsync(
                TimeSpan.FromSeconds(1),
                async () =>
                {
                    guildRoleActor.Tell(
                        registerNewRole,
                        probe.Ref);
                    await probe.ExpectMsgAsync<IError>();
                });
        }

        [Fact(Timeout = 2_000)]
        public async Task SaveDataInDatabase_WhenRegisteringRole()
        {
            // Arrange
            RegisterNewRole? registerNewRole = fix.Create<RegisterNewRole>();
            await guildRoleActor.Ask(registerNewRole);

            Within(
                TimeSpan.FromSeconds(1),
                () =>
                {
                    rolesRepositoryMock
                        .Received(1)
                        .InsertRole(
                            Arg.Is<GuildRole>(
                                gr =>
                                    gr.RoleId == registerNewRole.RoleId &&
                                    gr.GuildId == registerNewRole.GuildId &&
                                    gr.RoleLevel == registerNewRole.RoleLevel));
                });
        }

        [Fact(Timeout = 2_000)]
        public async Task RemoveDataInDatabase_WhenRemovingRole()
        {
            // Arrange
            RegisterNewRole registerNewRole = fix.Create<RegisterNewRole>() with { RoleLevel = UserLevel.Admin };
            await guildRoleActor.TryAsk(registerNewRole);
            DeleteRole deleteRole = new(
                registerNewRole.GuildId,
                registerNewRole.RoleId);
            await guildRoleActor.TryAsk(deleteRole);
            GetRoleLevel getRole = new(
                deleteRole.GuildId,
                deleteRole.RoleId);

            await WithinAsync(
                TimeSpan.FromSeconds(1),
                async () =>
                {
                    GetRoleLevelResponse? response = await guildRoleActor.Ask<GetRoleLevelResponse>(getRole);

                    Assert.Equal(
                        UserLevel.User,
                        response.RoleLevel);

                    await rolesRepositoryMock
                        .Received(1)
                        .DeleteRole(
                            registerNewRole.GuildId,
                            registerNewRole.RoleId);
                });
        }
    }
}