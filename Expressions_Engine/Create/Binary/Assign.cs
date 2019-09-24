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
        [Input("left","An Expression that represents the variable/property being assigned")]
        [Input("right","An Expression that represents the value to assign")]
        [Output("A BinaryExpression that represents an assignment")]
        [Description("Creates a BinaryExpression that represents an assignment")]
        public static BinaryExpression Assign(Expression left, Expression right)
        {
            return Expression.Assign(left, right);
        }
    }
}
