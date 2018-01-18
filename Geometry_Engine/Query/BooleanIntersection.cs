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

        //public static List<Polyline> BooleanIntersection(this Polyline region, Polyline curve)   //TODO: Add method for List<polyline> that uses bolean union to join the curves first 
        //{
        //    List<Point> regionCorners = region.ControlPoints;
        //    List<Point> outCtrlPts = curve.ControlPoints;
        //    outCtrlPts.RemoveAt(outCtrlPts.Count - 1);
        //    Plane plane = BH.Engine.Geometry.Create.Plane(regionCorners[0], regionCorners[1], regionCorners[2]);

        //    if (regionCorners.Count < 4 || outCtrlPts.Count < 3) throw new ArgumentException("The polylines needs to consist of at least 3 lines");

        //    if (!regionCorners.IsInPlane(plane) || !outCtrlPts.IsInPlane(plane)) return new List<Polyline>();

        //    List<Plane> regionEdgePlanes = new List<Plane>();
        //    Point C = regionCorners.Average();
        //    Vector normal = plane.Normal;

        //    for (int i = 1; i < regionCorners.Count; i++)
        //    {
        //        Vector edge = regionCorners[i - 1] - regionCorners[i];
        //        regionEdgePlanes.Add(new Plane { Origin = regionCorners[i - 1], Normal = edge.CrossProduct(normal) });
        //    }
        //    //TODO: Now using planes to find intersection. This is done because finding intersection between one infinite line and one finite 
        //    //is not inplemented. NEeds to be changed once the method for that has been inplemented. 

        //    for (int i = 0; i < regionEdgePlanes.Count; i++)
        //    {
        //        Plane regionEdgePlane = regionEdgePlanes[i];
        //        List<Point> inCtrlPts = new List<Point>();
        //        inCtrlPts.AddRange(outCtrlPts);
        //        outCtrlPts.Clear();
        //        if (inCtrlPts.Count == 0) return new List<Polyline>();
        //        Point S = inCtrlPts.Last();

        //        for (int j = 0; j < inCtrlPts.Count; j++)
        //        {
        //            Point E = inCtrlPts[j];

        //            if ((new Line { Start = C, End = E }).PlaneIntersection(regionEdgePlane) == null)
        //            {
        //                if ((new Line { Start = C, End = S }).PlaneIntersection(regionEdgePlane) != null) outCtrlPts.Add(new Line { Start = S, End = E }.PlaneIntersection(regionEdgePlane));
        //                outCtrlPts.Add(E);
        //            }
        //            else if ((new Line { Start = C, End = S }).PlaneIntersection(regionEdgePlane) == null) outCtrlPts.Add(new Line { Start = S, End = E }.PlaneIntersection(regionEdgePlane));
        //            S = E;
        //        }
        //    }
        //    if (outCtrlPts.Count == 0) return new List<Polyline>();
        //    else
        //    {
        //        outCtrlPts.Add(outCtrlPts.First());
        //        List<Polyline> polyLineList = new List<Polyline>() { new Polyline { ControlPoints = outCtrlPts } };
        //        return polyLineList;
        //    }
        //}

        public static List<Polyline> BooleanIntersection(this Polyline region, Polyline curve)
        {
            List<Point> regVert = region.ControlPoints;
            List<Line> regEdges = new List<Line>();
            for (int i = 1; i < regVert.Count; i++) regEdges.Add(Create.Line(regVert[i - 1], regVert[i]));
            List<Point> crvVert = curve.ControlPoints;
            List<Line> crvEdges = new List<Line>();
            for (int i = 1; i < crvVert.Count; i++) crvEdges.Add(Create.Line(crvVert[i - 1], crvVert[i]));

            //double w = 1;
            //int k = 0;
            //while (w == 1)           
            //{                
            //    w = 0;
            //    for (int i = 1; i < crvVert.Count; i++)
            //    {
            //        Vector v1 = Create.Vector(crvVert[i - 1].X - regVert[k].X, crvVert[i - 1].Y - regVert[k].Y, crvVert[i - 1].Z - regVert[k].Z);
            //        Vector v2 = Create.Vector(crvVert[i].X - regVert[k].X, crvVert[i].Y - regVert[k].Y, crvVert[i].Z - regVert[k].Z);
            //        w += v1.Angle(v2);
            //    }
            //    k++;
            //    w = w / (2 * Math.PI);
            //}
            //k--;
            //if (k!=0)
            //{
            //    int vertCount = regVert.Count;
            //    int edgeCount = vertCount - 1;
            //    regVert.AddRange(regVert.GetRange(1, regVert.Count - 1));
            //    regVert = regVert.GetRange(k, vertCount);
            //    regEdges.AddRange(regEdges);
            //    regEdges = regEdges.GetRange(k, edgeCount);
            //}            

            List<int> crossCrvIndicies = new List<int>();
            List<Point> crvClipVert = new List<Point> { crvVert[0] };

            int a = 1;
            for (int i = 0; i < crvEdges.Count; i++)
            {                
                for (int j = 0; j < regEdges.Count; j++)
                {
                    
                    if ((LineIntersection(crvEdges[i], regEdges[j]) != null))
                    {                       
                        crvClipVert.Add(LineIntersection(crvEdges[i], regEdges[j]));
                        crossCrvIndicies.Add(i+a);
                        a++;
                    }
                }
                crvClipVert.Add(crvVert[i+1]);
            }

            a = 1;
            List<int> crossRegIndicies = new List<int>();
            List<Point> regClipVert = new List<Point> { regVert[0] };

            for (int i = 0; i < regEdges.Count; i++)
            {
                for (int j = 0; j < crvEdges.Count; j++)
                {
                    if ((LineIntersection(regEdges[i], crvEdges[j]) != null))
                    {
                        regClipVert.Add(LineIntersection(regEdges[i], crvEdges[j]));
                        crossRegIndicies.Add(i+a);
                        a++;
                    }
                }
                regClipVert.Add(regVert[i+1]);
            }

            int vertListLength = regClipVert.Count;
            int indexListLength = crossCrvIndicies.Count;

            Point searchPt = crvClipVert[crossCrvIndicies[1]];
            int searchIndex = regClipVert.IndexOf(regClipVert.ClosestPoint(searchPt));
            int firstIndex = crossRegIndicies.IndexOf(searchIndex);
            //int firstIndex = crossRegIndicies.IndexOf(regClipVert.IndexOf(crvClipVert[crossCrvIndicies[1]]));
            List<int> wrapList = new List<int>();
            for (int i = 0; i < indexListLength; i++) wrapList.Add(crossRegIndicies[i] + vertListLength-1);            
            crossRegIndicies.AddRange(wrapList);
            regClipVert.AddRange(regClipVert.GetRange(1, vertListLength-1));
                        
            List<int> orderedRegIndicies = new List<int>();
            orderedRegIndicies.AddRange(crossRegIndicies.GetRange(firstIndex, indexListLength));         

            List<Polyline> outList = new List<Polyline>();
            for (int i = 0; i < indexListLength-1; i=+2)
            {
                List<Point> currPlPts = new List<Point>();
                currPlPts.AddRange(crvClipVert.Skip(crossCrvIndicies[i]).Take(crossCrvIndicies[i+1] - crossCrvIndicies[i]+1));
                currPlPts.AddRange(regClipVert.Skip(orderedRegIndicies[i]+1).Take(orderedRegIndicies[i+1] - orderedRegIndicies[i]));
                //currPlPts.Add(currPlPts.First());
                outList.Add(Create.Polyline(currPlPts));
            }
            return outList;       
                       
        }
    }
}