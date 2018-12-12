using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;


using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Building> Buildings(this List<IBHoMObject> bhomObjects)
        {
            List<Building> spaces = new List<Building>();

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is Building)
                    spaces.Add(obj as Building);
            }

            return spaces;
        }
    }
}