using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Fundamentals
{
    public static class Converter
    {
        public static EitherAsync<IError, TTarget> ConvertExt<TTarget>(this object source)
        {
            if (source is TTarget final)
            {
                return final;
            }

            return new ExceptionError(new Exception("Could not convert an object"));
        }
    }
}
