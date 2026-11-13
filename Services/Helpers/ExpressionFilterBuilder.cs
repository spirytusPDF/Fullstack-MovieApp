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
        public static async Task<Expression<Func<Movie, bool>>> BuildFilter<Movie, MovieParameters>(MovieParameters parameters)
        {
            var entityProps = typeof(Movie)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p=>p.Name, StringComparer.OrdinalIgnoreCase);
            var paramProps = typeof(MovieParameters).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var entityNode = Expression.Parameter(typeof(Movie), "x");

            Expression? result = null;

            foreach (var pProp in paramProps)
            {
                var value = pProp.GetValue(parameters);
                if (value == null) continue;
                if (value is string strvalue && string.IsNullOrEmpty(strvalue)) continue;

                if (!entityProps.TryGetValue(pProp.Name, out var entityPropMatch)) continue;

                var propertyAcess = Expression.Property(entityNode, entityPropMatch);
                //x.Property
                Expression comparison;

                if(entityPropMatch.PropertyType == typeof(string))
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
                    var targetType = Nullable.GetUnderlyingType(entityPropMatch.PropertyType) ?? entityPropMatch.PropertyType;
                    var convertedValue = Convert.ChangeType(value, targetType);
                    var valueConstant = Expression.Constant(convertedValue, entityPropMatch.PropertyType);

                    comparison = Expression.Equal(propertyAcess, valueConstant);
                }

                result = result == null ? comparison : Expression.AndAlso(result, comparison);

               

            }

             if (result == null) return x => true;

             return Expression.Lambda<Func<Movie, bool>>(result, entityNode);
        }
    
    }
}
