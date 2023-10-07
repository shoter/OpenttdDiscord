namespace OpenttdDiscord.Domain.Roles.UseCases
{
    public interface IDeleteRoleLevelUseCase
    {
        EitherAsyncUnit Execute(
            ulong guildId,
            ulong roleId);
    }
}