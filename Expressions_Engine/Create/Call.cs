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
        [Input("arguments","A list of expressions to pass as arguments")]
        [Input("method","The method to call")]
        [Output("A MethodCallExpression that represents a call to a method with the provided arguments")]
        [Description("Creates a MethodCallExpression that represents a call to a static method with the provided arguments")]
        public static MethodCallExpression Call(MethodInfo method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.Call(method, arguments);
        }

        [Input("arguments","A list of expressions to pass as arguments")]
        [Input("instance","The instance of the defining class on which to call the method (i.e. the value of `this`)")]
        [Input("method","The method to call")]
        [Output("A MethodCallExpression that represents a call to a method with the provided arguments")]
        [Description("Creates a MethodCallExpression that represents a call to an instance method on the provided instance with the provided arguments")]
        public static MethodCallExpression Call(Expression instance, MethodInfo method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.Call(instance, method, arguments);
        }

        [Input("arguments","A list of expressions to pass as arguments")]
        [Input("type","The Type that contains the method")]
        [Input("method","The name of the method to call")]
        [Output("A MethodCallExpression that represents a call to a method with the provided arguments")]
        [Description("Creates a MethodCallExpression that represents a call to a static method with the provided arguments")]
        public static MethodCallExpression Call(Type type, string method, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            Type[] types = arguments.Select(arg => arg.Type).ToArray();
            return Call(type.GetMethod(method, types), arguments);
        }

        [Input("arguments","A list of expressions to pass as arguments")]
        [Input("instance","The instance of the defining class on which to call the method (i.e. the value of `this`)")]
        [Input("method","The name of the method to call")]
        [Output("A MethodCallExpression that represents a call to a method with the provided arguments")]
        [Description("Creates a MethodCallExpression that represents a call to an instance method on the provided instance with the provided arguments")]
        public static MethodCallExpression Call(Expression instance, string method, List<Expression> arguments = null)
        {
            Type type = instance.Type;
            if (arguments == null) arguments = new List<Expression>();
            Type[] types = arguments.Select(arg => arg.Type).ToArray();
            return Call(instance, type.GetMethod(method, types), arguments);
        }
    }
}
