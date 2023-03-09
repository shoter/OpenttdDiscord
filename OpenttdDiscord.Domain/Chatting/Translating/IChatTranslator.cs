using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public interface IChatTranslator
    {
        Either<IError, string> FromDiscordToOttd(string input);

        Either<IError, string> FromOttdToDiscord(string input);
    }
}
