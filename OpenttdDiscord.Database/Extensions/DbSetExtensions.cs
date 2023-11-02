using System.Linq.Expressions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace OpenttdDiscord.Database.Extensions
{
    internal static class DbSetExtensions
    {
        internal static EitherAsyncUnit AddAsyncExt<TEntity>(
            this DbSet<TEntity> dbSet,
            TEntity entity)
            where TEntity : class => (from _1 in TryAsync(dbSet.AddAsync(entity))
            select Unit.Default).ToEitherAsyncError();
    }
}