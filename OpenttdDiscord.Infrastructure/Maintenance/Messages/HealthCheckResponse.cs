using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace OpenttdDiscord.Infrastructure.Maintenance.Messages;

public record HealthCheckResponse(IReadOnlyDictionary<string, HealthReportEntry> entries);