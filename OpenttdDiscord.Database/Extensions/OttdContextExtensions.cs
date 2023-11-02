using LanguageExt;

namespace OpenttdDiscord.Database.Extensions
{
    internal static class OttdContextExtensions
    {
        internal static EitherAsyncUnit SaveChangesAsyncExt(this OttdContext context) => TryAsync(
                from _1 in context.SaveChangesAsync()
                select Unit.Default)
            .ToEitherAsyncError();
    }
}
