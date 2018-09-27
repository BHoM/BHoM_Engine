using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> IdentifyOverlaps(this BuildingElement element, List<BuildingElement> elementsToCompare)
        {
            List<BuildingElement> overlappingElements = new List<BuildingElement>();

            foreach(BuildingElement be in elementsToCompare)
            {
                if(element.BHoM_Guid != be.BHoM_Guid && element.DoesBooleanIntersect(be))
                        overlappingElements.Add(be);
            }

            return overlappingElements;
        }

        /***************************************************/
    }
}
