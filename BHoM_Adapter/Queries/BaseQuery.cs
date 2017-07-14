using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Queries
{
    public class BaseQuery : IQuery
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        string Tag { get; set; }

        Type Type { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BaseQuery(Type type, string tag = "")
        {
            Type = type;
            Tag = Tag;
        }
    }
}
