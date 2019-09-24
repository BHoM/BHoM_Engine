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
        [Input("left","An Expression that represents the devidend")]
        [Input("right","An Expression that represents the devisor")]
        [Output("A BinaryExpression that represents a devision operation")]
            [Description("Creates a BinaryExpression that represents a division operation")]
        public static BinaryExpression Divide(Expression left, Expression right)
        {
            return Expression.Divide(left, right);
        }
    }
}
