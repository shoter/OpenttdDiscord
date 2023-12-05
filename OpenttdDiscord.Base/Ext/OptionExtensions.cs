using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public static class OptionExtensions
    {
        public static T Value<T>(this Option<T> option)
            => (T)option.Case!;

        public static EitherAsync<IError, T> ErrorWhenNone<T>(this Option<T> option, Func<IError> errorFunc)
        {
            if (option.IsNone)
            {
                return EitherAsync<IError, T>.Left(errorFunc());
            }

            return option.Value();
        }
    }
}
