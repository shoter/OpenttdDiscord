using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class ChatChannelActor : ReceiveActorBase
    {
        private readonly ulong chatChannelId;
        private ChatChannelActor(
            IServiceProvider serviceProvider,
            ulong chatChannelId) : base(serviceProvider)
        {
            this.chatChannelId = chatChannelId;
        }

        public static Props Create(IServiceProvider sp, ulong chatChannelId)
            => Props.Create(() => new ChatChannelActor(sp, chatChannelId));
    }
}
