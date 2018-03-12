using BH.oM.Environmental.Elements;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Space> AdjacentSpaces(this Space space, IEnumerable<Space> spaces, string buildingElementUniqueIdParameterName)
        {
            if (space == null || spaces == null)
                return null;

            List<Space> aResult = new List<Space>();
            foreach (BuildingElement aBuildingElement_Base in space.BuildingElements)
            {
                object aId_Base;

                if(aBuildingElement_Base.CustomData.TryGetValue(buildingElementUniqueIdParameterName, out aId_Base))
                {
                    foreach(Space aSpace in spaces)
                    {
                        foreach (BuildingElement aBuildingElement_Temp in aSpace.BuildingElements)
                        {
                            object aId_Temp;
                            if (aBuildingElement_Temp.CustomData.TryGetValue(buildingElementUniqueIdParameterName, out aId_Temp))
                            {
                                if (aId_Base == aId_Temp)
                                {
                                    aResult.Add(aSpace);
                                    break;
                                }  
                            }
                        }
                    } 
                }
            }
            return aResult;
        }

        public static List<Space> AdjacentSpaces(this BuildingElement buildingElement, IEnumerable<Space> spaces, string buildingElementUniqueIdParameterName)
        {
            if (buildingElement == null || spaces == null)
                return null;

            object aId_Base;

            if (buildingElement.CustomData.TryGetValue(buildingElementUniqueIdParameterName, out aId_Base))
            {
                List<Space> aResult = new List<Space>();

                foreach (Space aSpace in spaces)
                {
                    foreach (BuildingElement aBuildingElement in aSpace.BuildingElements)
                    {
                        object aId_Temp;
                        if (aBuildingElement.CustomData.TryGetValue(buildingElementUniqueIdParameterName, out aId_Temp))
                        {
                            if (aId_Base == aId_Temp)
                            {
                                aResult.Add(aSpace);
                                break;
                            }
                        }
                    }
                }

                return aResult;
            }

            return null;
        }

        /***************************************************/
    }
}

