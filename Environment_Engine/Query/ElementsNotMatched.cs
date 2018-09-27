using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> ElementsNotMatched(this List<BuildingElement> buildingElements, List<List<BuildingElement>> matchedToSpaces)
        {
            //Find the building elements that haven't been mapped yet
            List<BuildingElement> notYetMapped = new List<BuildingElement>();

            foreach (BuildingElement be in buildingElements)
            {
                if (!matchedToSpaces.IsContaining(be))
                    notYetMapped.Add(be);
            }

            return notYetMapped;
        }
    }
}
