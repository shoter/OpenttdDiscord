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
    internal class DeleteRoleRunner : OttdSlashCommandRunnerBase
    {
        private readonly IDeleteRoleLevelUseCase deleteRoleLevelUseCase;

        public DeleteRoleRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IDeleteRoleLevelUseCase deleteRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.deleteRoleLevelUseCase = deleteRoleLevelUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            var role = (IRole) options["role"];

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from _2 in deleteRoleLevelUseCase.Execute(
                    guildId,
                    role.Id)
                select new TextResponse("Role deleted!") as IInteractionResponse;
        }
    }
}