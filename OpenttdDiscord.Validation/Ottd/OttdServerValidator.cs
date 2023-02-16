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
                .Must(port => port >= 0 && port <= 65536);

            RuleFor(server => server.Name)
                .MinimumLength(1)
                .MaximumLength(30);

            RuleFor(server => server.AdminPortPassword)
                .Must(password => !string.IsNullOrWhiteSpace(password));

            RuleFor(server => server.Ip)
                .Must(ip =>
                {
                    return IPAddress.TryParse(ip, out var _);
                });
        }
    }
}
