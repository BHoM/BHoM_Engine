using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        public static LambdaExpression Lambda(Expression body, List<ParameterExpression> parameters = null)
        {
            if (parameters == null) parameters = new List<ParameterExpression>();
            return Expression.Lambda(body, parameters);
        }
    }
}
