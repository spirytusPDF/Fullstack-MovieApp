using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Services.Helpers
{
    public static class ExpressionFilterBuilder
    {
        public static async Task<Expression<Func<TEntity, bool>>> BuildFilter<TEntity, TParams>(TParams parameters)
        {
            var entityProps = typeof(TEntity)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p=>p.Name, StringComparer.OrdinalIgnoreCase);
            var paramProps = typeof(TParams).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var entityNode = Expression.Parameter(typeof(TEntity), "x");

            Expression? result = null;

            foreach (var pProp in paramProps)
            {
                var value = pProp.GetValue(parameters);
                if (value == null) continue;
                if (value is string strvalue && string.IsNullOrEmpty(strvalue)) continue;

                Expression propertyAcess;
                if (entityProps.TryGetValue(pProp.Name, out var entityPropMatch))
                {
                    propertyAcess = Expression.Property(entityNode, entityPropMatch);
                }
                else if (typeof(TEntity) == typeof(MovieSession)
                    && string.Equals(pProp.Name, "MovieTitle", StringComparison.OrdinalIgnoreCase))
                {
                    var movieProperty = typeof(MovieSession).GetProperty(nameof(MovieSession.Movie))!;
                    var titleProperty = typeof(Movie).GetProperty(nameof(Movie.Title))!;
                    propertyAcess = Expression.Property(Expression.Property(entityNode, movieProperty), titleProperty);
                }
                else
                {
                    continue;
                }

                //x.Property
                Expression comparison;

                if(propertyAcess.Type == typeof(string))
                {
                    var efLikeMethod = typeof(DbFunctionsExtensions)
                        .GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

                    var efFunction = Expression.Constant(EF.Functions);
                    var pattern = Expression.Constant($"%{value}%");

                    comparison = Expression.Call(efLikeMethod, efFunction, propertyAcess, pattern);

                    //SELECT * 
                    //FROM Movies AS m
                    //WHERE m.Title LIKE N'%Matrix%'
                }
                else
                {
                    var targetType = Nullable.GetUnderlyingType(propertyAcess.Type) ?? propertyAcess.Type;
                    var convertedValue = Convert.ChangeType(value, targetType);
                    var valueConstant = Expression.Constant(convertedValue, propertyAcess.Type);

                    comparison = Expression.Equal(propertyAcess, valueConstant);
                }

                result = result == null ? comparison : Expression.AndAlso(result, comparison);

               

            }

             if (result == null) return x => true;

             return Expression.Lambda<Func<TEntity, bool>>(result, entityNode);
        }
    
    }
}
