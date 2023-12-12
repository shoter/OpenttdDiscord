using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public interface IAkkaService
    {
        EitherAsync<IError, ActorSelection> SelectActor(string path);

        /// <summary>
        /// Uses <see cref="SelectActor"/> to select and actor and then asks it with a given message
        /// </summary>
        [SuppressMessage("Member Design",
                         "AV1115:Member or local function contains the word \'and\', which suggests doing multiple things",
                         Justification = "It is much easier to mock this method in tests. It does 2 things which is wrong, however testing ease is tremendous help.")]
        EitherAsync<IError, object> SelectAndAsk(string path, object message, TimeSpan? timeout = null);

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
