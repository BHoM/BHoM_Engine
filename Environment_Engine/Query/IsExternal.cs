using BH.oM.Environment.Elements;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsExternal(this BuildingElement buildingElement)
        {
            if (buildingElement == null || buildingElement.AdjacentSpaces == null)
                return false;

            return !(buildingElement.AdjacentSpaces.Count > 1);
        }

        /***************************************************/
    }
}


