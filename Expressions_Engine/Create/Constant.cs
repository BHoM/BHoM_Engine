using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        [Input("value", "The value to evaluate to")]
        [Output("A ConstantExpression that represents a constant value")]
        [Description("Creates a ConstantExpression that represents a constant value")]
        public static ConstantExpression Constant(object value)
        {
            return Expression.Constant(value);
        }
    }
}
