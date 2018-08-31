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
            List<BHE.BuildingElement> buildingElements = space.BuildingElements;
            BHG.PolyCurve curve = new oM.Geometry.PolyCurve();

            foreach(BHE.BuildingElement be in buildingElements)
            {
                if (BH.Engine.Environment.Query.Tilt(be.BuildingElementGeometry) == 180)
                    curve = be.BuildingElementGeometry.ICurve() as BHG.PolyCurve; //Is a floor
                //TODO: What if we have more than one floor?
            }

            BHG.Polyline floorBoundary = new BHG.Polyline() { ControlPoints = curve.ControlPoints() };

            if (floorBoundary.ControlPoints.Count < 3)
                return null;

            return floorBoundary;
        }
    }
}
