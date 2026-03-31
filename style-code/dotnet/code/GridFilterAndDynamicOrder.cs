using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PersonalCodeWiki.ZCYX
{
    // Adapted from GridFilterExtensions: keep filter shaping and dynamic ordering.
    public static class GridFilterAndDynamicOrder
    {
        public static IQueryable<Project> ApplyFilter(this IQueryable<Project> query, ProjectSearchDTO filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.ProjectCode))
            {
                query = query.Where(x => filter.ProjectCode == x.ProjectCode);
            }

            if (!string.IsNullOrWhiteSpace(filter.ProjectName))
            {
                query = query.Where(x => filter.ProjectName == x.Name);
            }

            if (filter.ProjectType != null)
            {
                query = query.Where(x => filter.ProjectType == x.ProjectType);
            }

            if (filter.Scale != null)
            {
                query = query.Where(x => filter.Scale == x.Scale);
            }

            if (filter.Locations != null && filter.Locations.Any())
            {
                query = query.Where(x => filter.Locations.Contains(x.Location.LocationId));
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy) && !string.IsNullOrWhiteSpace(filter.OrderBy))
            {
                query = query.OrderByDynamic(filter.SortBy, filter.OrderBy.ToUpper() == "DESC");
            }
            else
            {
                query = query.OrderByDescending(obj => obj.CreateTime);
            }

            return query;
        }

        private static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, bool descending)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var orderCommand = descending ? "OrderByDescending" : "OrderBy";

            PropertyInfo property;
            Expression propertyAccess = null;
            var propName = sortColumn;

            if (propName.Contains('.'))
            {
                var childProperties = propName.Split('.');
                property = typeof(T).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (property != null)
                {
                    propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    for (var i = 1; i < childProperties.Length; i++)
                    {
                        property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (property != null)
                        {
                            propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                        }
                    }
                }
            }
            else
            {
                property = typeof(T).GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (property != null)
                {
                    propertyAccess = Expression.MakeMemberAccess(parameter, property);
                }
            }

            if (property == null)
            {
                return query;
            }

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(
                typeof(Queryable),
                orderCommand,
                new[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
