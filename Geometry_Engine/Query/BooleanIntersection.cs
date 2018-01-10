using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Polylines                ****/
        /***************************************************/

        public static List<Polyline> BooleanIntersection(this Polyline region, Polyline curve)   //TODO: Add method for List<polyline> that uses bolean union to join the curves first 
        {
            List<Point> regionCorners = region.ControlPoints;
            List<Point> outCtrlPts = curve.ControlPoints;
            outCtrlPts.RemoveAt(outCtrlPts.Count - 1);
            Plane plane = BH.Engine.Geometry.Create.Plane(regionCorners[0], regionCorners[1], regionCorners[2]);

            if (regionCorners.Count < 4 || outCtrlPts.Count < 3) throw new NotImplementedException("The polylines needs to consist of at least 3 lines");

            if (!regionCorners.IsInPlane(plane) || !outCtrlPts.IsInPlane(plane)) throw new NotImplementedException("The polylines needs to be in the same plane");

            List<Plane> regionEdgePlanes = new List<Plane>();
            Point C = regionCorners.Average();
            Vector normal = plane.Normal;

            for (int i = 1; i < regionCorners.Count; i++)
            {
                Vector edge = regionCorners[i - 1] - regionCorners[i];
                regionEdgePlanes.Add(new Plane { Origin = regionCorners[i - 1], Normal = edge.CrossProduct(normal) });
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

                    if ((new Line { Start = C, End = E }).PlaneIntersection(regionEdgePlane) == null)
                    {
                        if ((new Line { Start = C, End = S }).PlaneIntersection(regionEdgePlane) != null) outCtrlPts.Add(new Line { Start = S, End = E }.PlaneIntersection(regionEdgePlane));
                        outCtrlPts.Add(E);
                    }
                    else if ((new Line { Start = C, End = S }).PlaneIntersection(regionEdgePlane) == null) outCtrlPts.Add(new Line { Start = S, End = E }.PlaneIntersection(regionEdgePlane));
                    S = E;
                }
            }
            if (outCtrlPts.Count == 0) return new List<Polyline>() { new Polyline() };
            else
            {
                outCtrlPts.Add(outCtrlPts.First());
                List<Polyline> polyLineList = new List<Polyline>() { new Polyline { ControlPoints = outCtrlPts } };
                return polyLineList;
            }
        }

        /***************************************************/  
    }
}
