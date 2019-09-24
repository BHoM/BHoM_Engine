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
        [Input("left","An Expression to use as the left side of the greater-than comparison")]
        [Input("right","An Expression to use as the right side of the greater-than comparison")]
        [Output("A BinaryExpression that represents a greater-than comparison operation")]
        [Description("Creates a BinaryExpression that represents a greater-than comparison operation")]
        public static BinaryExpression GreaterThan(Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }
    }
}
