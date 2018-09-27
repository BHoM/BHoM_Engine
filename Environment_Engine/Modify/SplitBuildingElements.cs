using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<BuildingElement> SplitBuildingElements(this List<BuildingElement> elementsToSplit)
        {
            //Go through all building elements and compare to see if any should be split into smaller building elements
            List<BuildingElement> rtn = new List<BuildingElement>();

            for(int x = 0; x < elementsToSplit.Count; x++)
            {
                List<BuildingElement> overlappingElements = elementsToSplit[x].IdentifyOverlaps(elementsToSplit);
                if (overlappingElements.Count == 0) rtn.Add(elementsToSplit[x]);
                else
                {
                    //This element overlaps with some elements in the list - split them up into new elements

                }
            }

            return rtn;
        }
    }
}