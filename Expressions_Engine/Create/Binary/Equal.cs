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
        [Input("left","An Expression to use as the left side of the equality comparison")]
        [Input("right","An Expression to use as the right side of the equality comparison")]
        [Output("A BinaryExpression that represents a equality comparison operation")]
        [Description("Creates a BinaryExpression that represents a equality comparison operation")]
        public static BinaryExpression Equal(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
    }
}
