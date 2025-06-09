using System.Linq.Expressions;

namespace TimeTile.API.Common.Api.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplySorting<T, TSortEnum>(
            this IQueryable<T> query, 
            string? sortBy, 
            bool descending,
            TSortEnum defaultSortField,
            Dictionary<TSortEnum, Expression<Func<T, object>>> sortSelectors) where TSortEnum : struct, Enum
        {
            if (string.IsNullOrEmpty(sortBy) ||
                !Enum.TryParse<TSortEnum>(sortBy, true, out var sortField) ||
                !sortSelectors.TryGetValue(sortField, out var sortExpression))
            {
                sortExpression = sortSelectors[defaultSortField];
            }

            return descending
                ? query.OrderByDescending(sortExpression)
                : query.OrderBy(sortExpression);
        }
    }
}
