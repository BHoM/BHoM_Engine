using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHG.Point> UnmatchedElementPoints(BHE.Space space, double tolerance = BHG.Tolerance.MacroDistance)
        {
            List<BHG.Point> nonMatchingPoints = new List<oM.Geometry.Point>();

            foreach(BHE.BuildingElement be in space.BuildingElements)
            {
                if (be == null) continue;

                foreach(BHG.Point pt in be.BuildingElementGeometry.ICurve().ICollapseToPolyline(BHG.Tolerance.Angle).IDiscontinuityPoints())
                {
                    List<BHE.BuildingElement> elementCompare = space.BuildingElements.Where(x => x != null).ToList().FindAll(x => x.BHoM_Guid != be.BHoM_Guid);

                    BHE.BuildingElement matchingBE = elementCompare.Find(x => x.BuildingElementGeometry.ICurve().ICollapseToPolyline(BHG.Tolerance.Angle).DiscontinuityPoints().ClosestDistance(new List<BHG.Point>() { pt }) < tolerance && (x.BHoM_Guid != be.BHoM_Guid));

                    if (matchingBE == null)
                        nonMatchingPoints.Add(pt);
                }
            }

            return nonMatchingPoints;
        }
    }
}
