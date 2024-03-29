using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class RegisterRoleRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRegisterRoleUseCase registerRoleUseCase;

        public RegisterRoleRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IRegisterRoleUseCase registerRoleUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.registerRoleUseCase = registerRoleUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            IRole? role = options["role"] as IRole;
            long roleLevel = (long) options["role-level"];

            if (role == null)
            {
                throw new ArgumentException(
                    options["role"]
                        .GetType()
                        .Name);
            }

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from _1 in registerRoleUseCase.Execute(
                    guildId,
                    role,
                    (UserLevel) roleLevel)
                select new TextResponse("Role has been registered") as IInteractionResponse;
        }
    }
}