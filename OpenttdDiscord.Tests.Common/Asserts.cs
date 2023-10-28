using FluentAssertions.Extensions;

namespace OpenttdDiscord.Tests.Common
{
    public static class Asserts
    {
        public static Task Within(
            Action assertAction,
            int numberOfTries = 10) => Within(
            1.Seconds(),
            assertAction);
        public static async Task Within(TimeSpan withinTime, Action assertAction,
                                  int numberOfTries = 10)
        {
            TimeSpan interval = withinTime / numberOfTries;
            Exception? lastException = null;

            for (int i = 0; i < numberOfTries; ++i)
            {
                try
                {
                    assertAction();
                }
                catch (Exception e)
                {
                    await Task.Delay(interval);
                    lastException = e;
                    continue;
                }

                return;
            }

            throw new Exception(
                "Within failed",
                lastException);
        }
    }
}
