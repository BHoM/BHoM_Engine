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
        public static MethodCallExpression Call(MethodInfo method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.Call(method, arguments);
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.Call(instance, method, arguments);
        }

        public static MethodCallExpression Call(Type type, string method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            Type[] types = arguments.Select(arg => arg.Type).ToArray();
            return Call(type.GetMethod(method, types), arguments);
        }

        public static MethodCallExpression Call(Expression instance, Type type, string method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            Type[] types = arguments.Select(arg => arg.Type).ToArray();
            return Call(instance, type.GetMethod(method, types), arguments);
        }
    }
}
