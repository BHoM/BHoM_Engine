using System;
using System.Collections.Generic;
using BH.oM.Environmental.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this Building building, Space space)
        {
            if (building == null || space == null)
                return null;

            List<BuildingElement> aResult = new List<BuildingElement>();
            foreach(BuildingElement aBuildingElement in building.BuildingElements)
            {
                if(aBuildingElement.AdjacentSpaces != null && aBuildingElement.AdjacentSpaces.Count > 0)
                {
                    Guid aGuid = aBuildingElement.AdjacentSpaces.Find(x => x == space.BHoM_Guid);
                    if(aGuid != null && aGuid != Guid.Empty)
                    {
                        Space aSpace = building.Spaces.Find(x => x.BHoM_Guid == aGuid);
                        if (aSpace != null)
                            aResult.Add(aBuildingElement);
                    }
                }
            }

            return aResult;
        }

        /***************************************************/
    }
}

