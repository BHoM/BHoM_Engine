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
        [Input("left","An Expression to use as the left side of the OR operation")]
        [Input("right","An Expression to use as the right side of the OR operation")]
        [Output("A BinaryExpression that represents a boolean OR operation")]
        [Description("Creates a BinaryExpression that represents a boolean OR operation")]
        public static BinaryExpression Or(Expression left, Expression right)
        {
            return Expression.OrElse(left, right);
        }
    }
}
