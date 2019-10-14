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
        [Input("obj","The object to get the property from")]
        [Input("property","The name of the property")]
        [Output("A MethodCallExpression that represents a call to PropertyValue for the object supplied")]
        [Description("Creates a MethodCallExpression that represents a call to PropertyValue for the object supplied")]
        public static MethodCallExpression GetProperty(Expression obj, Expression property)
        {
            return Call(typeof(Reflection.Query), "PropertyValue", new List<Expression> { obj, property });
        }

        [Input("obj","The object to get the property from")]
        [Input("property","The name of the property")]
        [Output("A MethodCallExpression that represents a call to PropertyValue for the object supplied")]
        [Description("Creates a MethodCallExpression that represents a call to PropertyValue for the object supplied")]
        public static MethodCallExpression GetProperty(Expression obj, string property)
        {
            return GetProperty(obj, Expression.Constant(property));
        }

        public static LambdaExpression GetProperty(string property)
        {
            ParameterExpression param = Expression.Parameter(typeof(object), "obj");
            return Expression.Lambda(GetProperty(param, Expression.Constant(property)), param);
        }
    }
}
