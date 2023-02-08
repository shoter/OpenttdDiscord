using FluentValidation;
using OpenttdDiscord.Domain.Servers;
using System.Net;

namespace OpenttdDiscord.Validation.Ottd
{
    public class OttdServerValidator : AbstractValidator<OttdServer>
    {
        public OttdServerValidator()
        {
            RuleFor(server => server.AdminPort)
                .Must(port => !port.HasValue || port >= 0 && port <= 65536);

            RuleFor(server => server.PublicPort)
                .Must(port => !port.HasValue || port >= 0 && port <= 65536);

            RuleFor(server => server.Name)
                .MinimumLength(1)
                .MaximumLength(30);

            RuleFor(server => server.AdminPortPassword)
                .Must(password => !string.IsNullOrWhiteSpace(password))
                .When(server => server.AdminPort.HasValue);

            RuleFor(server => server.AdminPort)
                .Must((server, adminPort, _) => server.PublicPort != adminPort)
                .When(server => server.AdminPort.HasValue);

            RuleFor(server => server.Ip)
                .Must(ip =>
                {
                    return IPAddress.TryParse(ip, out var _);
                });
        }
    }
}
