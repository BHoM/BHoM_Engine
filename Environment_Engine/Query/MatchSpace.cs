using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space MatchSpace(this List<Space> spaces, Point pt)
        {
            Space s = null;

            foreach(Space space in spaces)
            {
                double distToThisSpace = space.Location.Distance(pt);
                double distToCurrSpace = (s == null ? 1e10 : s.Location.Distance(pt));

                if (distToThisSpace < distToCurrSpace)
                    s = space;
            }

            return s;
        }
    }
}