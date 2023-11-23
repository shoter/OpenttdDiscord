using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

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

        public EitherAsync<IError, TExpectedMessage> SelectAndAsk<TExpectedMessage>(
            string path,
            object message,
            TimeSpan? timeout = null) =>
                from actor in SelectActor(path)
                from response in actor.TryAsk<TExpectedMessage>(message, timeout)
                select response;

        public void NotifyAboutAkkaStart()
        {
            isAkkaStared = true;
        }

        public EitherAsync<IError, ActorSelection> SelectActor(string path) => TryAsync(
                async () =>
                {
                    while (isAkkaStared == false)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }

                    return actorSystem.ActorSelection(path);
                })
            .ToEitherAsyncError();

        public EitherAsyncUnit ExecuteServerAction(ExecuteServerAction executeAction)
        {
            return
                from selection in SelectActor(MainActors.Paths.Guilds)
                from _1 in selection.TellExt(executeAction)
                    .ToAsync()
                select Unit.Default;
        }
    }
}