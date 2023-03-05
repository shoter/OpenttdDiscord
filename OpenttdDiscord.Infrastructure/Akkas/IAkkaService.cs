using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public interface IAkkaService
    {
        EitherAsync<IError, ActorSelection> SelectActor(string path);

        void NotifyAboutAkkaStart();
    }
}
