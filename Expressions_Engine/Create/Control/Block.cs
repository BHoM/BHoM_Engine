using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        [Input("expressions","A list of expressions in side the block to excecute in sequence")]
        [Output("A BlockExpression that contains the given expressions")]
        [Description("Creates a BlockExpression that contains the given expressions and contains no variables")]
        public static BlockExpression Block(List<Expression> expressions)
        {
            return Expression.Block(expressions);
        }

        [Input("expressions","A list of expressions in side the block to excecute in sequence")]
        [Input("variables","A list of variables to declare inside the block's scope")]
        [Output("A BlockExpression that contains the given expressions and the given variables")]
        [Description("Creates a BlockExpression that contains the given expressions and the given variables")]
        public static BlockExpression Block(List<ParameterExpression> variables, List<Expression> expressions)
        {
            return Expression.Block(variables, expressions);
        }
    }
}
