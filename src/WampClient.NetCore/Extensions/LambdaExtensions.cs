using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WampClient.Core.Extensions
{
    public static class LambdaExtensions
    {
        public static void SetPropertyValue<T>(this T target, Expression<Func<T, object>> member, object value)
        {
            var memberSelectorExpression = member.Body as MemberExpression;
            var property = memberSelectorExpression?.Member as PropertyInfo;
            property?.SetValue(target, value, null);
        }
    }
}