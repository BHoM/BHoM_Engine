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
        
        [Input("expression", "The expression to be converted")]
        [Input("type", "The Type to attempt to convert to")]
        [Output("An Expression that represents a type conversion")]
        [Description("Creates an Expression that represents a type conversion")]
        public static Expression Convert(Expression expression, Type type)
        {
            return Expression.Convert(expression, type);
        }
    }
}
