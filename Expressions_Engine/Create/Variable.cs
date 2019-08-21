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
        public static ParameterExpression Variable(Type type, string name = null)
        {
            if (name == null) return Expression.Variable(type);
            return Expression.Variable(type, name);
        }
    }
}
