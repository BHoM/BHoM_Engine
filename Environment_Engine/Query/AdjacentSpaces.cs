using BH.oM.Environmental.Elements;
using System;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Space> AdjacentSpaces(this Building building, Space space)
        {
            return AdjacentSpaces(building, space.BHoM_Guid);
        }

        /***************************************************/

        public static List<Space> AdjacentSpaces(this Building building, Guid spaceGuid)
        {
            if (building == null || spaceGuid == null || spaceGuid == Guid.Empty)
                return null;

            List<Space> aResult = new List<Space>();
            foreach (BuildingElement aBuildingElement in building.BuildingElements)
            {
                if (aBuildingElement.AdjacentSpaces != null && aBuildingElement.AdjacentSpaces.Count > 0)
                {
                    Guid aGuid = aBuildingElement.AdjacentSpaces.Find(x => x == spaceGuid);
                    if (aGuid != null && aGuid != Guid.Empty)
                    {
                        Space aSpace = building.Spaces.Find(x => x.BHoM_Guid == aGuid);
                        if (aSpace != null)
                        {
                            foreach (Guid aGuid_Temp in aBuildingElement.AdjacentSpaces)
                            {
                                if (aGuid_Temp != aSpace.BHoM_Guid)
                                {
                                    Space aSpace_Temp = building.Spaces.Find(x => x.BHoM_Guid == aGuid);
                                    if (aSpace_Temp != null)
                                        aResult.Add(aSpace_Temp);
                                }

                            }
                        }
                    }
                }
            }

            return aResult;
        }

        /***************************************************/

        public static List<Space> AdjacentSpaces(this BuildingElement buildingElement, IEnumerable<Space> spaces)
        {
            if (buildingElement == null || spaces == null)
                return null;

            List<Space> aResult = new List<Space>();
            foreach (Guid aGuid in buildingElement.AdjacentSpaces)
            {
                Space aSpace = null;
                foreach (Space aSpace_Temp in spaces)
                {
                    if(aSpace_Temp.BHoM_Guid == aGuid)
                    {
                        aSpace = aSpace_Temp;
                        break;
                    }
                }
                aResult.Add(aSpace);
            }

            return aResult;
        }

        /***************************************************/
    }
}

