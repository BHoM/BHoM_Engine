using BH.oM.Environment.Elements;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsInternal(this BuildingElement buildingElement)
        {
            if (buildingElement == null)
                return false;

            return buildingElement.BuildingElementProperties.BuildingElementType != BuildingElementType.Window && buildingElement.BuildingElementProperties.BuildingElementType != BuildingElementType.Roof; //TODO: Put a more robust check of whether the element is internal or not in...
        }

        /***************************************************/
    }
}


