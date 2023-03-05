using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Reporting.UseCases
{
    public interface IRegisterReportChannelUseCase
    {
        EitherAsyncUnit Execute(User user, ReportChannel reportChannel);
    }
}
