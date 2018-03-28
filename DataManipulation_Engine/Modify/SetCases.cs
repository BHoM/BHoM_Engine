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

        public static FilterQuery SetCases(this FilterQuery query, List<object> cases)
        {
            FilterQuery clone = query.Clone();
            clone.Equalities["Cases"] = cases;
            return clone;
        }

        /***************************************************/
    }
}
