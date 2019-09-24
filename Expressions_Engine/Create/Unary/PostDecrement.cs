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
        [Input("expression","The expression to decrement")]
        [Output("A UnaryExpression that decrements an expression and returns its previous value")]
        [Description("Creates a UnaryExpression that decrements an expression and returns its previous value")]
        public static UnaryExpression PostDecrement(Expression expression)
        {
            return Expression.PostDecrementAssign(expression);
        }
    }
}
