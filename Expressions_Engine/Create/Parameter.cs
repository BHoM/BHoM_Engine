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
        public static ParameterExpression Parameter(Type type, string name = null)
        {
            if (name == null) return Expression.Parameter(type);
            return Expression.Parameter(type, name);
        }
    }
}
