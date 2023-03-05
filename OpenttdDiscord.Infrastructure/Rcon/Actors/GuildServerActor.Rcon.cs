using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Rcon.Actors;
using OpenttdDiscord.Infrastructure.Rcon.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor
    {
        private IListRconChannelsUseCase listRconChannelsUseCase = default!;
        private Dictionary<ulong, IActorRef> rconChannels = new();

        private void RconConstructor()
        {
            this.listRconChannelsUseCase = SP.GetRequiredService<IListRconChannelsUseCase>();
        }

        private void RconReady()
        {
            Receive<RegisterNewRconChannel>(RegisterNewRconChannel);
            Receive<UnregisterRconChannel>(UnregisterRconChannel);
        }

        private void RegisterNewRconChannel(RegisterNewRconChannel msg)
            => CreateNewRconActor(msg.RconChannel);

        private async Task RconInit()
        {
            List<RconChannel> channels = (await listRconChannelsUseCase.Execute(User.Master, server.Id))
                .ThrowIfError()
                .Right();

            foreach (var channel in channels)
            {
                CreateNewRconActor(channel);
            }
        }

        private void UnregisterRconChannel(UnregisterRconChannel msg)
        {
            if(!rconChannels.TryGetValue(msg.channelId, out IActorRef? actor))
            {
                logger.LogWarning($"An attempt was made to remove rcon channel {msg} but no rcon actor was found");
                return;
            }

            actor.GracefulStop(TimeSpan.FromSeconds(1));
            rconChannels.Remove(msg.channelId);
        }

        private void CreateNewRconActor(RconChannel channel)
        {
            var rconActor = Context.ActorOf(RconChannelActor.Create(SP, channel, server, client), $"rcon-{channel.ChannelId}");
            rconChannels.Add(channel.ChannelId, rconActor);
        }
    }
}
