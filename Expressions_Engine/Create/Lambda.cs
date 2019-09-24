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

        [Input("parameters","A list of parameters to pass to the body.")]
        [Input("body","An expression that represents the body of the Lambda function. The ParameterExpressions passed in 'parameters' should be reused within this body expression.")]
        [Output("A LambdaExpression that can be compiled to a delegate")]
        [Description("Creates a LambdaExpression that can be compiled to a delegate")]
        public static LambdaExpression Lambda(Expression body, List<ParameterExpression> parameters = null)
        {
            if (parameters == null) parameters = new List<ParameterExpression>();
            return Expression.Lambda(body, parameters);
        }
    }
}
