using BH.oM.Base;
using BH.oM.DataManipulation.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataManipulation
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<IBHoMObject> Filter(this IEnumerable<IBHoMObject> objects, FilterQuery query)
        {
            return Compute.FilterData(query, objects);
        }

        /***************************************************/
    }
}
