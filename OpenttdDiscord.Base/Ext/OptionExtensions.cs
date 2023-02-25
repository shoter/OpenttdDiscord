using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public static class OptionExtensions
    {
        public static T Value<T>(this Option<T> option)
            => (T)option.Case!;
    }
}
