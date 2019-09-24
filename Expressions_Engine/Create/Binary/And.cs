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
        [Input("left","An Expression to use as the left side of the AND operation")]
        [Input("right","An Expression to use as the right side of the AND operation")]
        [Output("A BinaryExpression that represents a boolean AND operation")]
        [Description("Creates a BinaryExpression that represents a boolean AND operation")]
        public static BinaryExpression And(Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }
    }
}
