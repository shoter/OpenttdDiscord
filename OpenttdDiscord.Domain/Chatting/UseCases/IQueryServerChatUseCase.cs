using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Chatting.UseCases
{
    public interface IQueryServerChatUseCase
    {
        EitherAsync<IError, IReadOnlyList<string>> Execute(User user, Guid serverId, ulong guildId);
    }
}
