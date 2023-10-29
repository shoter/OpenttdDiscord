using Akka.Actor;
using Akka.TestKit;
using LanguageExt;

namespace OpenttdDiscord.Tests.Common.Akkas
{
    /// <summary>
    /// Replies to sender with <see cref="Unit.Default"/> for every message received.
    /// </summary>
    public class UnitAutoPilot : AutoPilot
    {
        public override AutoPilot Run(
            IActorRef sender,
            object _)
        {
            sender.Tell(Unit.Default);
            return AutoPilot.KeepRunning;
        }
    }
}