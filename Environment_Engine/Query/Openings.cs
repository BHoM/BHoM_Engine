using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Opening> Openings(this List<IBHoMObject> bhomObjects)
        {
            List<Opening> openings = new List<Opening>();

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is Opening)
                    openings.Add(obj as Opening);
            }

            return openings;
        }

        public static List<Opening> Openings(this List<BuildingElement> elements)
        {
            List<Opening> openings = new List<Opening>();

            foreach(BuildingElement be in elements)
            {
                List<Opening> beOpenings = be.Openings;
                foreach(Opening o in beOpenings)
                {
                    Opening opInList = openings.Where(x => x.BHoM_Guid == o.BHoM_Guid).FirstOrDefault();
                    if (opInList == null)
                        openings.Add(o);
                }
            }

            return openings;
        }
    }
}