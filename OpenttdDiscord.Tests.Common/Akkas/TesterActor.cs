using Akka.Actor;
using LanguageExt;

namespace OpenttdDiscord.Tests.Common.Akkas
{
    public class TesterActor : ReceiveActor
    {
        private List<TesterHandle> Handles { get; } = new();

        public static Props Create() => Props.Create(() => new TesterActor());

        public TesterActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<TesterHandle>(
                handle =>
                {
                    Handles.Add(handle);
                    Sender.Tell(Unit.Default);
                });
            Receive<object>(HandleMessage);
        }

        private void HandleMessage(object message)
        {
            for (int i = 0; i < Handles.Count; ++i)
            {
                TesterHandle handler = Handles[i];
                if (handler.IsExpectedMessage(message))
                {
                    object returnMessage = handler.CreateResponse(message);
                    Sender.Tell(returnMessage);

                    if (handler.DisposeAfterExpected)
                    {
                        Handles.RemoveAt(i);
                    }

                    return;
                }

                if (handler.IsBlocking)
                {
                    return;
                }
            }
        }
    }
}