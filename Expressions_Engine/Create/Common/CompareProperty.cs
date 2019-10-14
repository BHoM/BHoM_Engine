using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        public static LambdaExpression CompareProperty(string property, LambdaExpression comparitor)
        {
            ParameterExpression a = Expression.Parameter(typeof(object));
            ParameterExpression b = Expression.Parameter(typeof(object));

            Expression prop_a = GetProperty(a, property);
            Expression prop_b = GetProperty(b, property);

            return Expression.Lambda(Expression.Invoke(comparitor, prop_a, prop_b), a, b);
        }
    }
}
