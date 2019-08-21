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
        public static Expression While(Expression condition, Expression body)
        {
            LabelTarget breakLabel = Expression.Label();
            return Expression.Loop(               // while(true) {
                Expression.IfThenElse(            //
                    condition,                    //   if(condition) {
                    body,                         //     body;
                    Expression.Break(breakLabel)  //   } else { break; }
                ),                                // }
                breakLabel
            );
        }
    }
}
