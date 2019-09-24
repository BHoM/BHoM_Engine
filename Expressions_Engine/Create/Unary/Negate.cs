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
        [Input("expression","The expression to negate")]
        [Output("A UnaryExpression that represents an arithmetic negation operation")]
        [Description("Creates a UnaryExpression that represents an arithmetic negation operation")]
        public static UnaryExpression Negate(Expression expression)
        {
            return Expression.Negate(expression);
        }
    }
}
