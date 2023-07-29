using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public interface IAkkaService
    {
        EitherAsync<IError, ActorSelection> SelectActor(string path);

        void NotifyAboutAkkaStart();

        /// <summary>
        /// Executes given action against ottd server.
        /// </summary>
        /// <remarks>
        /// All information regarding which server it should be executed are within <paramref name="executeAction"/>
        /// </remarks>
        EitherAsyncUnit ExecuteServerAction(ExecuteServerAction executeAction);
    }
}
