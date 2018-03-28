using BH.oM.DataManipulation.Queries;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace BH.Engine.DataManipulation
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FilterQuery SetObjectIds(this FilterQuery query, List<object> cases)
        {
            FilterQuery clone = query.Clone();
            clone.Equalities["ObjectIds"] = cases;
            return clone;
        }

        /***************************************************/
    }
}
