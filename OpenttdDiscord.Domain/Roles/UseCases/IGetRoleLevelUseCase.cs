using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Roles.UseCases
{
    public interface IGetRoleLevelUseCase
    {
        EitherAsync<IError, UserLevel> Execute(IUser user);
    }
}