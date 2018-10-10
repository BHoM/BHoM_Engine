using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<BuildingElement>> AdjacentSpaces(this BuildingElement element, List<List<BuildingElement>> spaces)
        {
            //Get the lists which contain this building element
            return spaces.Where(x => x.Where(y => y.BHoM_Guid == element.BHoM_Guid).Count() > 0).ToList();
        }

        public static List<Space> AdjacentSpaces(this BuildingElement element, List<List<BuildingElement>> besAsSpace, List<Space> spaces)
        {
            List<Space> rtn = new List<oM.Environment.Elements.Space>();

            List<Point> spaces1 = element.AdjacentSpaces(besAsSpace).SpaceCentres();

            foreach(Point p in spaces1)
            {
                Space add = spaces.MatchSpace(p);
                if (add != null)
                    rtn.Add(add);
            }

            return rtn;
        }
    }
}