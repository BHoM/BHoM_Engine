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

        public static List<BuildingElement> CullDuplicates(this List<BuildingElement> elements)
        {
            //Go through each building element and compare vertices and centre points - if there is a matching element, remove it
            for(int x = 0; x < elements.Count; x++)
            {
                if (elements[x] == null) continue;

                for(int y = x + 1; y < elements.Count; y++)
                {
                    if (elements[x].IsIdentical(elements[y]))
                        elements[y] = null;
                }
            }

            return elements.Where(x => x != null).ToList();
        }
    }
}
