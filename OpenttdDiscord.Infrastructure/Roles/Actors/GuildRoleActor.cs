using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Actors
{
    public class GuildRoleActor : ReceiveActorBase
    {
        private readonly IRolesRepository rolesRepository;

        private readonly ulong guildId;

        private ExtDictionary<ulong, GuildRole> guildRoles = new();

        public GuildRoleActor(
            IServiceProvider serviceProvider,
            ulong guildId)
            : base(serviceProvider)
        {
            rolesRepository = SP.GetRequiredService<IRolesRepository>();
            this.guildId = guildId;
            Ready();
            InitGuildRoleActor()
                .AsTask()
                .Wait();
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            ulong guildId) => Props.Create(
            () => new GuildRoleActor(
                serviceProvider,
                guildId));

        private void Ready()
        {
            ReceiveEitherAsync<UpsertRole>(UpsertRole);
            ReceiveEitherAsync<DeleteRole>(DeleteRole);
            Receive<GetRoleLevel>(GetRoleLevel);
        }

        private EitherAsyncUnit InitGuildRoleActor()
        {
            return
                from roles in rolesRepository.GetRoles(guildId)
                from _1 in AddRoles(roles)
                    .ToAsync()
                select Unit.Default;
        }

        private EitherUnit AddRoles(List<GuildRole> roles)
        {
            foreach (var role in roles)
            {
                guildRoles.Add(
                    role.RoleId,
                    role);
            }

            return Unit.Default;
        }

        private EitherAsyncUnit UpsertRole(UpsertRole msg)
        {
            var guildRole = new GuildRole(
                msg.GuildId,
                msg.RoleId,
                msg.RoleLevel);

            var sender = Sender;

            if (guildRoles.ContainsKey(msg.RoleId))
            {
                return
                    from _1 in rolesRepository.UpdateRole(guildRole)
                    from _2 in guildRoles.ReplaceExt(
                            msg.RoleId,
                            guildRole)
                        .ToAsync()
                    from _3 in sender.TellExt(Unit.Default)
                        .ToAsync()
                    select Unit.Default;
            }

            return
                from _1 in rolesRepository.InsertRole(guildRole)
                from _2 in guildRoles.AddExt(
                        msg.RoleId,
                        guildRole)
                    .ToAsync()
                from _3 in sender.TellExt(Unit.Default)
                    .ToAsync()
                select Unit.Default;
        }

        private void GetRoleLevel(GetRoleLevel msg)
        {
            var userLevel = UserLevel.User;
            foreach (var roleId in msg.RoleIds)
            {
                var guildRoleMaybe = guildRoles.MaybeGetValue(roleId);

                guildRoleMaybe.IfSome(
                    roleLevel =>
                    {
                        if (roleLevel.RoleLevel > userLevel)
                        {
                            userLevel = roleLevel.RoleLevel;
                        }
                    });
            }

            Sender.Tell(new GetRoleLevelResponse(userLevel));
        }

        private EitherAsyncUnit DeleteRole(DeleteRole msg)
        {
            var sender = Sender;
            return
                from _1 in rolesRepository.DeleteRole(
                    msg.GuildId,
                    msg.RoleId)
                from _2 in guildRoles.RemoveExt(msg.RoleId)
                    .ToAsync()
                from _3 in sender.TellExt(Unit.Default)
                    .ToAsync()
                select Unit.Default;
        }
    }
}