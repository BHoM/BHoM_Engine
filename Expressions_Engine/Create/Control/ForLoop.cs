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
        public static Expression ForLoop (Expression init, Expression condition, Expression post, Expression body)
        {
            LabelTarget breakLabel = Expression.Label();
            return Expression.Block(
                init,                                  // init;
                Expression.Loop(                       // while(true) {
                    Expression.IfThenElse(
                        condition,                     //   if (condition) {
                        Expression.Block(body, post),  //     body; post;
                        Expression.Break(breakLabel)   //   } else { break; }
                    ),                                 // }
                    breakLabel
                )
            );

        }
    }
}
