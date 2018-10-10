using System;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Base;

using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this List<IBHoMObject> bhomObjects)
        {
            List<BuildingElement> bes = new List<BuildingElement>();

            foreach(IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }

        public static List<BuildingElement> UniqueBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach(List<BuildingElement> lst in elements)
            {
                foreach(BuildingElement be in lst)
                {
                    BuildingElement beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }
    }
}

