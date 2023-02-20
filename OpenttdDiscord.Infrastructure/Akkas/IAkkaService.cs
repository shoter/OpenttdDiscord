using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public interface IAkkaService
    {
        Task<ActorSelection> SelectActor(string path);

        void NotifyAboutAkkaStart();
    }
}
