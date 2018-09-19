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

        public static bool NormalAwayFromSpace(this BHE.BuildingElement buildingElement, BHE.Space space)
        {
            BHG.Polyline bound = new BHG.Polyline() { ControlPoints = buildingElement.PanelCurve.IControlPoints() };

            return NormalAwayFromSpace(bound, space);
        }

        /***************************************************/

        public static bool NormalAwayFromSpace(this BHG.Polyline pline, BHE.Space space)
        {
            List<BHG.Point> centrePtList = new List<BHG.Point>();
            BHG.Point centrePt = pline.Centre();
            centrePtList.Add(centrePt);

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!BH.Engine.Geometry.Query.IsClockwise(pline, plane.Normal))
                plane.Normal = -plane.Normal;

            if (!BH.Engine.Geometry.Query.IsContaining(pline, centrePtList, false))
            {
                BHG.Point pointOnLine = BH.Engine.Geometry.Query.ClosestPoint(pline, centrePt);
                BHG.Vector vector = new BHG.Vector();
                if (BH.Engine.Geometry.Query.Distance(pointOnLine, centrePt) > BH.oM.Geometry.Tolerance.MicroDistance)
                    vector = pointOnLine - centrePt;
                else
                {
                    BHG.Line line = BH.Engine.Geometry.Query.GetLineSegment(pline, pointOnLine);
                    vector = ((line.Start - line.End).Normalise()).CrossProduct(plane.Normal);
                }

                centrePt = BH.Engine.Geometry.Modify.Translate(pointOnLine, BH.Engine.Geometry.Modify.Normalise(vector) * 0.001);
            }

            //Move centrepoint along the normal.
            if (BH.Engine.Environment.Query.IsContaining(space, centrePt.Translate(plane.Normal * 0.01)))
                return false;
            else
                return true;
        }
    }
}
