using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.Messages;

internal record RegisterStatusMonitor(StatusMonitor StatusMonitor) : IOttdServerMessage
{
    public Guid ServerId => StatusMonitor.ServerId;
}
