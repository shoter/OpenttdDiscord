using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Reporting.UseCases;

public interface IUnregisterReportChannelUseCase
{
    EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId);
}
