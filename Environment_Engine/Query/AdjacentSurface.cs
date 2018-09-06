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

        public static List<BHE.BuildingElement> AdjacentSurface(this BHE.BuildingElement bHoMBuildingElement, BHE.Building building)
        {
            if (bHoMBuildingElement == null || bHoMBuildingElement.AdjacentSpaces.Count == 0)
                return null;

            BHG.Plane plane = bHoMBuildingElement.BuildingElementGeometry.ICurve().IFitPlane();
            List<BHG.Polyline> plines = new List<BHG.Polyline>();

            List<BHE.BuildingElement> besInPlane = new List<BHE.BuildingElement>();
            BHG.Polyline pline = new BHG.Polyline() { ControlPoints = bHoMBuildingElement.BuildingElementGeometry.ICurve().IControlPoints() };

            if (pline.IsClosed())
                plines.Add(pline);

            //Find Space
            BHE.Space space = building.Spaces.Find(x => x.BHoM_Guid.ToString() == bHoMBuildingElement.AdjacentSpaces.First().ToString());

            //Find the other panels in the same plane that intersects with the polyline
            foreach (BHE.BuildingElement be in space.BuildingElements)
            {
                if (be.BuildingElementGeometry.ICurve().IIsInPlane(plane) && be.BHoM_Guid.ToString() != bHoMBuildingElement.BHoM_Guid.ToString())
                {
                    if (be.BuildingElementGeometry.ICurve().IIsClosed())
                    {
                        plines.Add(be.BuildingElementGeometry.ICurve().ICollapseToPolyline(BHG.Tolerance.MacroDistance));

                        if (plines.Count == 2 && BH.Engine.Geometry.Compute.BooleanUnion(plines).Count == 1) //If the two polylines intersect they can be merged into one srf by BooleanUnion
                            besInPlane.Add(be);
                    }
                }
            }

            return besInPlane;
        }
    }
}
