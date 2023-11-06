using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.Akkas.Message;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Actors
{
    public class AutoReplyActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCase;
        private readonly ulong guildId;
        private readonly Guid serverId;

        private Option<IActorRef> welcomeActor = Option<IActorRef>.None;
        private Dictionary<string, IActorRef> instanceActors = new();

        public AutoReplyActor(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            this.getWelcomeMessageUseCase = SP.GetRequiredService<IGetWelcomeMessageUseCase>();
            this.guildId = guildId;
            this.serverId = serverId;

            instanceActors.Add(
                "dupa",
                Context.ActorOf(
                    AutoReplyInstanceActor.Create(
                        SP,
                        new AutoReply(
                            "!papiez",
                            @"
no i ja się pytam człowieku dumny ty jesteś z siebie zdajesz sobie sprawę z tego co robisz?masz ty wogóle rozum i godnośc człowieka?ja nie wiem ale żałosny typek z ciebie ,chyba nie pomyślałes nawet co robisz i kogo obrażasz ,możesz sobie obrażac tych co na to zasłużyli sobie ale nie naszego papieża polaka naszego rodaka wielką osobę ,i tak wyjątkowa i ważną bo to nie jest ktoś tam taki sobie że możesz go sobie wyśmiać bo tak ci się podoba nie wiem w jakiej ty się wychowałes rodzinie ale chyba ty nie wiem nie rozumiesz co to jest wiara .jeśli myslisz że jestes wspaniały to jestes zwykłym czubkiem którego ktoś nie odizolował jeszcze od społeczeństwa ,nie wiem co w tym jest takie śmieszne ale czepcie się stalina albo hitlera albo innych zwyrodnialców a nie czepiacie się takiej świętej osoby jak papież jan paweł 2 .jak można wogóle publicznie zamieszczac takie zdięcia na forach internetowych?ja się pytam kto powinien za to odpowiedziec bo chyba widac że do koscioła nie chodzi jak jestes nie wiem ateistą albo wierzysz w jakies sekty czy wogóle jestes może ty sługą szatana a nie będziesz z papieża robił takiego ,to ty chyba jestes jakis nie wiem co sie jarasz pomiotami szatana .wez pomyśl sobie ile papież zrobił ,on był kimś a ty kim jestes żeby z niego sobie robić kpiny co? kto dał ci prawo obrażac wogóle papieża naszego ?pomyślałes wogóle nad tym że to nie jest osoba taka sobie że ją wyśmieje i mnie będa wszyscy chwalic? wez dziecko naprawdę jestes jakis psycholek bo w przeciwieństwie do ciebie to papież jest autorytetem dla mnie a ty to nie wiem czyim możesz być autorytetem chyba takich samych jakiś głupków jak ty którzy nie wiedza co to kosciół i religia ,widac że się nie modlisz i nie chodzisz na religie do szkoły ,widac nie szanujesz religii to nie wiem jak chcesz to sobie wez swoje zdięcie wstaw ciekawe czy byś sie odważył .naprawdę wezta się dzieci zastanówcie co wy roicie bo nie macie widac pojęcia o tym kim był papież jan paweł2 jak nie jestescie w pełni rozwinięte umysłowo to się nie zabierajcie za taką osobę jak ojciec swięty bo to świadczy o tym że nie macie chyba w domu krzyża ani jednego obraza świętego nie chodzi tutaj o kosciół mnie ale wogóle ogólnie o zasady wiary żeby mieć jakąs godnosc bo papież nikogo nie obrażał a ty za co go obrażasz co? no powiedz za co obrażasz taką osobę jak ojciec święty ?brak mnie słów ale jakbyś miał pojęcie chociaz i sięgnął po pismo święte i poczytał sobie to może byś się odmienił .nie wiem idz do kościoła bo widac już dawno szatan jest w tobie człowieku ,nie lubisz kościoła to chociaż siedz cicho i nie obrażaj innych ludzi .
",
                            AutoReplyAction.ResetCompany),
                        this.client)));

            Ready();
            Self.Tell(new InitializeActor());
            parent.Tell(new SubscribeToAdminEvents(Self));
        }

        protected override void PostStop()
        {
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
            base.PostStop();
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client) => Props.Create(
            () => new AutoReplyActor(
                serviceProvider,
                guildId,
                serverId,
                client));

        private void Ready()
        {
            Receive<UpdateWelcomeMessage>(UpsertWelcomeMessage);
            ReceiveRedirect<AdminClientJoinEvent>(() => welcomeActor);
            ReceiveRedirect<AdminChatMessageEvent>(() => instanceActors.Values);
            ReceiveEitherAsync<InitializeActor>(InitializeActor);
            ReceiveIgnore<IAdminEvent>();
        }

        private EitherAsyncUnit InitializeActor(InitializeActor _)
        {
            var context = Context;
            return
                from message in getWelcomeMessageUseCase.Execute(
                    guildId,
                    serverId)
                select message.IfSome(msg => TryToInitializeActor(msg, context));
        }

        private Unit TryToInitializeActor(WelcomeMessage welcomeMessage, IActorContext context)
        {
            if (welcomeActor.IsSome)
            {
                return Unit.Default;
            }

            welcomeActor = CreateWelcomeActor(welcomeMessage.Content, context);
            return Unit.Default;
        }

        private void UpsertWelcomeMessage(UpdateWelcomeMessage msg)
        {
            if (welcomeActor.IsSome)
            {
                welcomeActor.ForwardExt(msg);
                Sender.Tell(Unit.Default);
                return;
            }

            welcomeActor = CreateWelcomeActor(msg.Content, Context);
            Sender.Tell(Unit.Default);
        }

        private Option<IActorRef> CreateWelcomeActor(string content, IActorContext context)
        {
            IActorRef actor = context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    content));

            return Some(actor);
        }
    }
}