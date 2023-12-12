namespace OpenttdDiscord.Base.Basics
{
    public static class DateTimeExtensions
    {
        public static string ToTime(this DateTime time) => $"{time:HH:mm:ss.fff}";
    }
}