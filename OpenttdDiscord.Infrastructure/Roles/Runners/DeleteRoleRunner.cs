using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

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

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            throw new NotImplementedException();
        }
    }
}