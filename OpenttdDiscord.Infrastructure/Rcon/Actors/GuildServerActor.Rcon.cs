using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Rcon.Actors;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor
    {
        private IGetRconChannelUseCase getRconChannelUseCase = default!;
        private Dictionary<ulong, IActorRef> rconChannels = new();

        private void RconConstructor()
        {
            this.getRconChannelUseCase = SP.GetRequiredService<IGetRconChannelUseCase>();
        }

        private void RconReady()
        {
        }

        private async Task RconInit()
        {
            List<RconChannel> channels = (await getRconChannelUseCase.Execute(User.Master, server.Id))
                .ThrowIfError()
                .Right();

            foreach (var channel in channels)
            {
                var rconActor = Context.ActorOf(RconChannelActor.Create(SP, channel, server, client), $"rcon-{channel.ChannelId}");
                rconChannels.Add(channel.ChannelId, rconActor);
            }
        }
    }
}
