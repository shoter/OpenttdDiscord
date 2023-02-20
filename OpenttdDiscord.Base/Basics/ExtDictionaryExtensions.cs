namespace OpenttdDiscord.Base.Basics
{
    public static class ExtDictionaryExtensions
    {
        public static ExtDictionary<TKey, TElement> ToExtDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TKey : notnull
            => new ExtDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector));
    }
}
