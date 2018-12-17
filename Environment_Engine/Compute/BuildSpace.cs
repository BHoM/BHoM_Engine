using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static List<BuildingElement> BuildSpace(this List<BuildingElement> elements, string spaceName)
        {
            return elements.Where(x => x.CustomData["SpaceID"].ToString() == spaceName || x.CustomData["AdjacentSpaceID"].ToString() == spaceName).ToList();
        }

        public static List<List<BuildingElement>> BuildSpaces_Revit(this List<BuildingElement> elements, List<string> spaceNames)
        {
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            foreach (String s in spaceNames)
                spaces.Add(elements.BuildSpace(s));

            return spaces;
        }
    }
}
