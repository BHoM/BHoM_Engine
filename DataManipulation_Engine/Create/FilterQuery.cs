using BH.oM.DataManipulation.Queries;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BH.Engine.DataManipulation
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FilterQuery FilterQuery(Type type = null, string tag = "")
        {
            return new FilterQuery { Type = type, Tag = tag };
        }

        /***************************************************/
        public static FilterQuery FilterQuery(Type type, Dictionary<string, object> equalities, string tag = "")
        {
            return new FilterQuery { Type = type, Tag = tag, Equalities = equalities };
        }

        /***************************************************/

        public static FilterQuery FilterQuery(Type type, IEnumerable<object> cases = null, IEnumerable<object> objectIds = null, string tag = "")
        {
            Dictionary<string, object> equalities = new Dictionary<string, object>();

            if (cases != null)
                equalities["Cases"] = cases.ToList();
            if (objectIds != null)
                equalities["ObjectIds"] = objectIds.ToList();

            return FilterQuery(type, equalities, tag);
        }

        /***************************************************/
    }
}
