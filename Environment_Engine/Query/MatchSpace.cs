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

        public static List<List<BuildingElement>> MatchSpaces(this List<List<BuildingElement>> elementsAsSpaces, List<Space> spaces)
        {
            foreach (List<BuildingElement> bes in elementsAsSpaces)
            {
                Space foundSp = null;

                foreach (Space s in spaces)
                {
                    if (s.Location == null)
                    {
                        foundSp = s; break;
                    }
                    if (!bes.IsContaining(s.Location))
                        continue;
                    else
                    {
                        foundSp = s;
                        break;
                    }
                }

                if (foundSp != null)
                {
                    foreach (BuildingElement be in bes)
                        be.CustomData.Add("Space_Custom_Data", foundSp.CustomData);
                }
            }

            return elementsAsSpaces;
        }
    }
}