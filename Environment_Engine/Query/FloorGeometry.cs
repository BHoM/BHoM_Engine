using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Environment;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BHE = BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHG.Polyline FloorGeometry(this BHE.Space space)
        {
            throw new NotImplementedException("Calculating the floor geometry in the space has not been implemented");
        }

        public static BHG.Polyline FloorGeometry(this List<BHE.BuildingElement> space)
        {
            BHE.BuildingElement floor = null;

            foreach(BHE.BuildingElement be in space)
            {
                if (BH.Engine.Environment.Query.Tilt(be) == 0)
                {
                    if(floor == null)
                        floor = be;
                    else
                    {
                        //Multiple elements could be a floor - assign the one with the lowest Z
                        if (floor.MinimumLevel() > be.MinimumLevel())
                            floor = be;
                    }
                }
            }

            if (floor == null) return null;

            BHG.Polyline floorGeometry = floor.PanelCurve as BHG.Polyline;

            if (floorGeometry.ControlPoints.Count < 3)
                return null;

            return floorGeometry;
        }
    }
}
