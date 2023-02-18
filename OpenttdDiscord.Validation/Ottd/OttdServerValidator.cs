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
                .Must(port => port >= 0 && port <= 65536)
                .WithMessage("Port must be a number between 0 and 65536!");

            RuleFor(server => server.Name)
                .MinimumLength(1)
                .MaximumLength(30)
                .WithMessage("Server name can be maximum 30 characters long!");

            RuleFor(server => server.AdminPortPassword)
                .Must(password => !string.IsNullOrWhiteSpace(password))
                .WithMessage("Password must be provided");

            RuleFor(server => server.Ip)
                .Must(ip =>
                {
                    return IPAddress.TryParse(ip, out var _);
                })
                .WithMessage("IP address must be a correct IPv4 or IPv6 address");
        }
    }
}
