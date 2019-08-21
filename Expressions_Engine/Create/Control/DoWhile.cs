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
        public static Expression DoWhile(Expression condition, Expression body)
        {
            LabelTarget breakLabel = Expression.Label();
            return Expression.Loop(                    // while(true)
                Expression.Block(                      // {
                    body,                              //   body;
                    Expression.IfThen(                 //
                        Expression.IsFalse(condition), //   if(!condition) {
                        Expression.Break(breakLabel)   //     break;
                    )                                  //   }
                ),                                     // }
                breakLabel
            );
        }
    }
}
