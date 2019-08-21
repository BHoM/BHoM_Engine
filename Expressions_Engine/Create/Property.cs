using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        public static Expression Property(Expression instance, string property)
        {
            return Expression.Property(instance, property);
        }
    }
}
