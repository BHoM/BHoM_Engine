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

        public static List<BuildingElement> BuildSpace_Revit(this List<BuildingElement> elements, string revitSpaceName)
        {
            return elements.Where(x => x.CustomData["Revit_spaceId"].ToString() == revitSpaceName || x.CustomData["Revit_adjacentSpaceId"].ToString() == revitSpaceName).ToList();
        }

        public static List<List<BuildingElement>> BuildSpaces_Revit(this List<BuildingElement> elements, List<string> revitSpaceNames)
        {
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            foreach (String s in revitSpaceNames)
                spaces.Add(elements.BuildSpace_Revit(s));

            return spaces;
        }
    }
}
