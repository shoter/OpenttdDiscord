using System.Linq.Expressions;
using LanguageExt;

namespace OpenttdDiscord.Database.Extensions
{
    internal static class IQueryableExtensions
    {
        internal static EitherAsync<IError, IQueryable<TSource>> WhereExt<TSource>(
            this IQueryable<TSource> queryable,
            Expression<Func<TSource, bool>> predicate) => (from result in TryAsync(queryable.Where(predicate))
            select result).ToEitherAsyncError();

        internal static EitherAsync<IError, Option<TSource>> FirstOptionalExt<TSource>(
            this IQueryable<TSource> queryable) => (from result in TryAsync(queryable.FirstOrDefault())
            select result == null ? Option<TSource>.None : Some(result)).ToEitherAsyncError();

        internal static EitherAsync<IError, Option<TSource>> FirstOptionalExt<TSource>(
            this IQueryable<TSource> queryable,
            Expression<Func<TSource, bool>> predicate) => (from result in TryAsync(queryable.FirstOrDefault(predicate))
            select result == null ? Option<TSource>.None : Some(result)).ToEitherAsyncError();

        internal static EitherAsync<IError, TSource> FirstExt<TSource>(
            this IQueryable<TSource> queryable) => (from result in TryAsync(queryable.FirstOrDefault())
            select result).ToEitherAsyncError();
    }
}