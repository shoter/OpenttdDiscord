using Akka.Actor;
using LanguageExt;
using LanguageExt.SomeHelp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Actors
{
    public class GuildRoleActor : ReceiveActorBase
    {
        private readonly IRolesRepository rolesRepository;

        private ExtDictionary<ulong, GuildRole> guildRoles = new();

        public GuildRoleActor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            rolesRepository = SP.GetRequiredService<IRolesRepository>();
            Ready();
        }

        public static Props Create(IServiceProvider serviceProvider) =>
            Props.Create(() => new GuildRoleActor(serviceProvider));

        private void Ready()
        {
            ReceiveEitherAsync<RegisterNewRole>(RegisterNewRole);
            ReceiveEitherAsync<DeleteRole>(DeleteRole);
            Receive<GetRoleLevel>(GetRoleLevel);
        }

        private EitherAsyncUnit RegisterNewRole(RegisterNewRole msg)
        {
            var guildRole = new GuildRole(
                msg.GuildId,
                msg.RoleId,
                msg.RoleLevel);

            return
                from _1 in rolesRepository.InsertRole(guildRole)
                from _2 in guildRoles.AddExt(msg.RoleId, guildRole).ToAsync()
                from _3 in Sender.TellExt(Unit.Default).ToAsync()
                select Unit.Default;
        }

        private void GetRoleLevel(GetRoleLevel msg)
        {
            var guildRoleMaybe = guildRoles.MaybeGetValue(msg.RoleId);

            guildRoleMaybe.BiIter(
                (guildRole) =>
                {
                    Sender.Tell(new GetRoleLevelResponse(guildRole.RoleLevel));
                },
                () =>
                {
                    Sender.Tell(new GetRoleLevelResponse(UserLevel.User));
                });
        }

        private EitherAsyncUnit DeleteRole(DeleteRole msg)
        {
            return
                from _1 in rolesRepository.DeleteRole(msg.GuildId, msg.RoleId)
                from _2 in guildRoles.RemoveExt(msg.RoleId).ToAsync()
                select Unit.Default;
        }
    }
}