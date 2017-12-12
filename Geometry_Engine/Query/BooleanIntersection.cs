using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Polylines                ****/
        /***************************************************/

        public static List<Polyline> GetBooleanIntersection(this Polyline region, Polyline curve)
        {
            List<Point> regionCorners = region.ControlPoints;
            List<Point> outCtrlPts = curve.ControlPoints;
            outCtrlPts.RemoveAt(outCtrlPts.Count - 1);
            Plane plane = BH.Engine.Geometry.Create.Plane(regionCorners[0], regionCorners[1], regionCorners[2]);

            if (regionCorners.Count < 4 || outCtrlPts.Count < 3) throw new NotImplementedException("The polylines needs to consist of at least 3 lines");

            if (!regionCorners.IsInPlane(plane) || !outCtrlPts.IsInPlane(plane)) throw new NotImplementedException("The polylines needs to be in the same plane");

            List<Plane> regionEdgePlanes = new List<Plane>();
            Point C = regionCorners.GetAverage();
            Vector normal = plane.Normal;

            for (int i = 1; i < regionCorners.Count; i++)
            {
                Vector edge = new Vector(new Point(regionCorners[i - 1] - regionCorners[i]));
                regionEdgePlanes.Add(new Plane(regionCorners[i - 1], edge.GetCrossProduct(normal)));
            }
            //TODO: Now using planes to find intersection. This is done because finding intersection between one infinite line and one finite 
            //is not inplemented. NEeds to be changed once the method for that has been inplemented. 

            for (int i = 0; i < regionEdgePlanes.Count; i++)
            {
                Plane regionEdgePlane = regionEdgePlanes[i];
                List<Point> inCtrlPts = new List<Point>();
                inCtrlPts.AddRange(outCtrlPts);
                outCtrlPts.Clear();
                Point S = inCtrlPts.Last();

                for (int j = 0; j < inCtrlPts.Count; j++)
                {
                    Point E = inCtrlPts[j];

                    if ((new Line(C, E)).GetIntersection(regionEdgePlane) == null)
                    {
                        if ((new Line(C, S)).GetIntersection(regionEdgePlane) != null) outCtrlPts.Add(new Line(S, E).GetIntersection(regionEdgePlane));
                        outCtrlPts.Add(E);
                    }
                    else if ((new Line(C, S)).GetIntersection(regionEdgePlane) == null) outCtrlPts.Add(new Line(S, E).GetIntersection(regionEdgePlane));
                    S = E;
                }
            }
            outCtrlPts.Add(outCtrlPts.First());
            List<Polyline> polyLineList = new List<Polyline>() { new Polyline(outCtrlPts) };
            return polyLineList;
        }

        //TODO: Add method for List<polyline> that uses bolean union to join the curves first 
    }
}
