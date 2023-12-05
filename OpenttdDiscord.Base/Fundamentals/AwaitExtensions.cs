using System.Runtime.CompilerServices;

namespace OpenttdDiscord.Base.Fundamentals
{
    public static class AwaitExtensions
    {
        public static TaskAwaiter GetAwaiter(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks)
                .GetAwaiter();
        }
    }
}