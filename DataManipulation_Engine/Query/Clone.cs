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

        public static FilterQuery Clone(this FilterQuery query)
        {
            return new FilterQuery() { Type = query.Type, Equalities = new Dictionary<string, object>(query.Equalities), Tag = query.Tag };
        }

        /***************************************************/
    }
}
