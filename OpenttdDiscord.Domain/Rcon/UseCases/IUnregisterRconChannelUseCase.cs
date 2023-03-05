using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Rcon.UseCases
{
    public interface IUnregisterRconChannelUseCase
    {
        EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId);
    }
}
