using LanguageExt;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Openttd
{
    public static class IAdminPortClientExtensions
    {
        public static EitherAsync<IError, ServerStatus> QueryServerStatusExt(this IAdminPortClient client) =>
            (from result in TryAsync(client.QueryServerStatus())
                select result).ToEitherAsyncError();
    }
}
