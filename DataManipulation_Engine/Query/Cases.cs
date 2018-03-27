using BH.oM.DataManipulation.Queries;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace BH.Engine.DataManipulation
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<object> Cases(this FilterQuery query)
        {
            object obj;

            if (query.Equalities.TryGetValue("Cases", out obj))
            {
                if (obj is IEnumerable)
                    return (obj as IEnumerable<object>).ToList();
            }

            return null;
        }

        /***************************************************/
    }
}
