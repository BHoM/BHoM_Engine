using BH.oM.DataManipulation.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataManipulation
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BatchQuery BatchQuery(IEnumerable<IQuery> queries)
        {
            return new BatchQuery { Queries = queries.ToList() };
        }

        /***************************************************/
    }
}
