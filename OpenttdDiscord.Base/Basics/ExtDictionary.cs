using LanguageExt;

namespace OpenttdDiscord.Base.Basics
{
    public class ExtDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        where TKey : notnull
    {
        public ExtDictionary(Dictionary<TKey, TValue> dictionary)
            : base(dictionary)
        { }

        public Option<TValue> MaybeGetValue(TKey key)
        {
            if(this.TryGetValue(key, out var value))
            {
                return value;
            }

            return Option<TValue>.None;
        }

        public TValue GetValue(TKey key) => this[key];

        public TAs GetValueAs<TAs>(TKey key)
            where TAs : TValue
            => (TAs)this[key] !;

        public Option<TAs> TryGetValueAs<TAs>(TKey key)
        {
            if (this.TryGetValue(key, out var value))
            {
                if(value is TAs ass)
                {
                    return ass;
                }
            }

            return Option<TAs>.None;
        }
    }
}
