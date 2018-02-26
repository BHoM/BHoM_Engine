using BH.oM.Base;
using BH.oM.DataManipulation.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataManipulation
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<IBHoMObject> FilterData(this FilterQuery query, IEnumerable<IBHoMObject> objects)
        {
            IEnumerable<IBHoMObject> result = objects;

            if (query.Tag != "")
                result = objects.Where(x => x.Tags.Contains(query.Tag));

            if (query.Type != null)
                result = result.Where(x => query.Type.IsAssignableFrom(x.GetType()));

            foreach (KeyValuePair<string, object> kvp in query.Equalities)
            {
                //TODO: Need to check the equalities as well
            }

            return result;
        }

        /***************************************************/
    }
}
