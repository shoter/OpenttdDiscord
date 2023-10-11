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
    public sealed class GuildRoleActorShould : BaseActorTestKit
    {
        private readonly IActorRef guildRoleActor = default!;
        private readonly IRolesRepository rolesRepositoryMock = Substitute.For<IRolesRepository>();

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            services.AddSingleton(rolesRepositoryMock);
        }

        public GuildRoleActorShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            guildRoleActor = ActorOf(
                GuildRoleActor.Create(
                    Sp,
                    12345));

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

            rolesRepositoryMock
                .GetRoles(default)
                .ReturnsForAnyArgs(new List<GuildRole>());
        }

        [Fact(Timeout = 2_000)]
        public async Task RegisterAndRetrieve_Role()
        {
            // Arrange
            UpsertRole? registerNewRole = fix.Create<UpsertRole>();
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
            UpsertRole? registerNewRole = fix.Create<UpsertRole>();
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
            UpsertRole? registerNewRole = fix.Create<UpsertRole>();
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
            UpsertRole upsertRole = fix.Create<UpsertRole>() with { RoleLevel = UserLevel.Admin };
            await guildRoleActor.TryAsk(upsertRole);
            DeleteRole deleteRole = new(
                upsertRole.GuildId,
                upsertRole.RoleId);
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
                            upsertRole.GuildId,
                            upsertRole.RoleId);
                });
        }
    }
}