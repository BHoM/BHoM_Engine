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
        public static NewExpression New(ConstructorInfo constructor, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.New(constructor, arguments);
        }

        public static NewExpression NewExpression(Type type, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            Type[] types = arguments.Select(arg => arg.Type).ToArray();
            return Expression.New(type.GetConstructor(types));
        }
    }
}
