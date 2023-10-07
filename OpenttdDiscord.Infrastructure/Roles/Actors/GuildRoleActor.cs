using Akka.Actor;
using LanguageExt;
using LanguageExt.SomeHelp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
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
            Self.Tell(new InitGuildRoleActor());
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            ulong guildId) => Props.Create(
            () => new GuildRoleActor(
                serviceProvider,
                guildId));

        private void Ready()
        {
            ReceiveEitherAsync<RegisterNewRole>(RegisterNewRole);
            ReceiveEitherAsync<DeleteRole>(DeleteRole);
            ReceiveEitherAsync<InitGuildRoleActor>(InitGuildRoleActor);
            Receive<GetRoleLevel>(GetRoleLevel);
        }

        private EitherAsyncUnit InitGuildRoleActor(InitGuildRoleActor _)
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

        private EitherAsyncUnit RegisterNewRole(RegisterNewRole msg)
        {
            var guildRole = new GuildRole(
                msg.GuildId,
                msg.RoleId,
                msg.RoleLevel);

            if (guildRoles.ContainsKey(msg.RoleId))
            {
                Sender.TellExt(new HumanReadableError("This role is already defined"));
                return Unit.Default;
            }

            var sender = Sender;
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