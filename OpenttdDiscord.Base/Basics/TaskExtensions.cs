using LanguageExt;

namespace OpenttdDiscord.Base.Basics
{
    public static class TaskExtensions
    {
        public static Unit RunAsync(this Task task)
        {
            return Unit.Default;
        }

        public static Unit RunAsync<T>(this Task<T> task)
        {
            return Unit.Default;
        }
    }
}
