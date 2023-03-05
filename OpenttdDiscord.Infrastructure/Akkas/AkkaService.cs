using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    internal class AkkaService : IAkkaService
    {
        private readonly ActorSystem actorSystem;

        private bool isAkkaStared = false;

        public AkkaService(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public void NotifyAboutAkkaStart()
        {
            isAkkaStared = true;
        }

        public EitherAsync<IError, ActorSelection> SelectActor(string path)
        => TryAsync(async () =>
        {
            while (isAkkaStared == false)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return actorSystem.ActorSelection(path);
        }).ToEitherAsyncError();
    }
}
