using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers.UseCases
{
    public interface IGetServerByNameUseCase
    {
        EitherAsync<IError, OttdServer> Execute(User user, string serverName, ulong guildId);
    }
}
