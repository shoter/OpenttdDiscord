using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using AutoFixture;
using FluentAssertions;
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
        private readonly IRolesRepository rolesRepositorySub = Substitute.For<IRolesRepository>();

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            services.AddSingleton(rolesRepositorySub);
        }

        public GuildRoleActorShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            guildRoleActor = ActorOf(
                GuildRoleActor.Create(
                    Sp,
                    12345));

            rolesRepositorySub
                .InsertRole(default!)
                .ReturnsForAnyArgs(Unit.Default);

            rolesRepositorySub
                .DeleteRole(
                    default!,
                    default!)
                .ReturnsForAnyArgs(Unit.Default);

            rolesRepositorySub
                .DeleteRole(default!)
                .ReturnsForAnyArgs(Unit.Default);

            rolesRepositorySub
                .GetRoles(default)
                .ReturnsForAnyArgs(new List<GuildRole>());

            rolesRepositorySub
                .UpdateRole(default!)
                .ReturnsForAnyArgs(Unit.Default);
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
        public async Task UpdateRole_AfterRegistration()
        {
            // Arrange
            UpsertRole? registerNewRole = fix.Create<UpsertRole>() with { RoleLevel = UserLevel.Moderator };
            guildRoleActor.Tell(
                registerNewRole,
                probe.Ref);
            probe.ExpectMsg<Unit>();

            await guildRoleActor.Ask(registerNewRole with { RoleLevel = UserLevel.Admin });

            await rolesRepositorySub
                .Received()
                .UpdateRole(
                    new GuildRole(
                        registerNewRole.GuildId,
                        registerNewRole.RoleId,
                        UserLevel.Admin
                    ));

            var response = await guildRoleActor.Ask<GetRoleLevelResponse>(
                new GetRoleLevel(
                    registerNewRole.GuildId,
                    registerNewRole.RoleId));

            Assert.Equal(
                UserLevel.Admin,
                response.RoleLevel);
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
                    rolesRepositorySub
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

                    await rolesRepositorySub
                        .Received(1)
                        .DeleteRole(
                            upsertRole.GuildId,
                            upsertRole.RoleId);
                });
        }
    }
}