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
        [Input("expression","The expression to test")]
        [Output("A UnaryExpression that returns if an expression evaluates to false")]
        [Description("Creates a UnaryExpression that returns if an expression evaluates to false")]
        public static UnaryExpression Not(Expression expression)
        {
            return Expression.IsFalse(expression);
        }
    }
}
