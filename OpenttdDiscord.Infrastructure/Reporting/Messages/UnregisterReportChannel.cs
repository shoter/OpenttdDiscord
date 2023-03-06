namespace OpenttdDiscord.Infrastructure.Reporting.Messages;

internal record UnregisterReportChannel(Guid ServerId, ulong guildId, ulong channelId);
