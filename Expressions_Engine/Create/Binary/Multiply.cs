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
        [Input("left","An Expression that represents the mutiplicand")]
        [Input("right","An Expression that represents the multiplier")]
        [Output("A BinaryExpression that represents a multiplication operation")]
        [Description("Creates a BinaryExpression that represents a multiplication operation")]
        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            return Expression.Multiply(left, right);
        }
    }
}
