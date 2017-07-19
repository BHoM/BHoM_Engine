using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Queries
{
    public class CustomQuery : IQuery
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Query { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public CustomQuery(string query = "")
        {
            Query = query;
        }
    }
}