using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static BHG.Polyline StoreyGeometry(this BH.oM.Architecture.Elements.Level level, List<BHE.Space> spaces)
        {
            /*List<BHE.Space> spacesAtLevel = spaces.FindAll(x => x.Level.Elevation == level.Elevation).ToList();

            if (spacesAtLevel.Count == 0)
                return null;

            List<BHE.BuildingElement> bHoMBuildingElement = spacesAtLevel.SelectMany(x => x.BuildingElements).ToList();
            List<BHG.Point> ctrlPoints = new List<BHG.Point>();

            foreach (BHE.BuildingElement element in bHoMBuildingElement)
            {
                foreach (BHG.Point pt in element.BuildingElementGeometry.ICurve().IControlPoints())
                {
                    if (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance))
                        ctrlPoints.Add(pt);

                }
            }

            return BH.Engine.Geometry.Create.ConvexHull(ctrlPoints.CullDuplicates());*/

            return new oM.Geometry.Polyline();
        }
    }
}
