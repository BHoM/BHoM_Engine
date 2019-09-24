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
        [Input("left","An Expression to use as the left side of the addition operation")]
        [Input("right","An Expression to use as the right side of the addition operation")]
        [Output("A BinaryExpression that represents an addition operation")]
        [Description("Creates a BinaryExpression that represents an addition operation")]
        public static BinaryExpression Add(Expression left, Expression right)
        {
            return Expression.Add(left, right);
        }
    }
}
