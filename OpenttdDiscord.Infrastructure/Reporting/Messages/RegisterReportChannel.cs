using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Reporting.Messages;

internal record RegisterReportChannel(ReportChannel ReportChannel) : IOttdServerMessage
{
    public Guid ServerId => ReportChannel.ServerId;
}
