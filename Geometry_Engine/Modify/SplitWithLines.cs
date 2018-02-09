using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Polyline> Split(this Polyline region, List<Line> refLines)
        {
            // Todo:
            // - smart application of graph theory should boost performance a lot

            List<Point> iInts = refLines.LineIntersections();
            List<Point> eInts = new List<Point>();
            List<Line> intLines = new List<Line>();
            foreach (Line rl in refLines)
            {
                List<Point> intPts = iInts.Select(p => p).ToList();
                List<Point> iPts = region.LineIntersections(rl);
                intPts.AddRange(iPts);
                eInts.AddRange(iPts);
                List<Line> splitRefLines = rl.SplitAtPoints( intPts);
                foreach (Line srl in splitRefLines)
                {
                    if (srl.Start.SquareDistance( srl.Start.ClosestPoint( intPts)) > Tolerance.SqrtDist || srl.End.SquareDistance( srl.End.ClosestPoint( intPts)) > Tolerance.SqrtDist) continue;
                    List<Point> cPt = new List<Point> { srl.ControlPoints().Average() };
                    if (region.IsContaining( cPt, false)) intLines.Add(srl);
                }
            }

            Vector normal = region.ControlPoints.FitPlane().Normal;
            List<Polyline> result = new List<Polyline>();
            if (intLines.Count > 0)
            {
                if (!region.IsClockwise(normal)) normal = normal.Reverse();
                List<Polyline> cutRegion = region.SplitAtPoints(eInts);
                List<Line> edges = new List<Line>();
                foreach (Polyline cr in cutRegion)
                {
                    edges.AddRange(cr.SubParts());
                }
                List<Line> newEdges = intLines.Select(l => l.Clone()).ToList();
                edges.AddRange(newEdges);
                edges.AddRange(newEdges.Select(e => e.Flip()).ToList());

                List<List<Line>> panelEdges = new List<List<Line>>();
                List<Vector> edgeDirections = edges.Select(e => e.Direction()).ToList();

                double revAngTol = Math.PI - Tolerance.Angle;
                while (edges.Count > 0)
                {
                    List<Line> le = new List<Line> { edges[0].Clone() };
                    Vector dir = edgeDirections[0].Clone();
                    Point ePt = edges[0].End;

                    while (le[0].Start.SquareDistance(ePt) > Tolerance.SqrtDist)
                    {
                        double maxAngle = -Math.PI;
                        int edgeId = -1;
                        for (int i = 1; i < edges.Count; i++)
                        {
                            Line l = edges[i];
                            double angle = dir.SignedAngle( edgeDirections[i], normal);
                            if (angle < revAngTol)
                            {
                                if (angle >= maxAngle && ePt.SquareDistance( l.Start) <= Tolerance.SqrtDist)
                                {
                                    maxAngle = angle;
                                    edgeId = i;
                                }
                            }
                        }
                        le.Add(edges[edgeId].Clone());
                        dir = edgeDirections[edgeId].Clone();
                        edges.RemoveAt(edgeId);
                        edgeDirections.RemoveAt(edgeId);
                        ePt = le.Last().End;
                    }
                    edges.RemoveAt(0);
                    edgeDirections.RemoveAt(0);
                    panelEdges.Add(le);
                }

                foreach (List<Line> po in panelEdges)
                {
                    List<Point> cPts = new List<Point> { po[0].Start };
                    foreach (Line l in po)
                    {
                        cPts.Add(l.End);
                    }
                    result.Add(new Polyline { ControlPoints = cPts });
                }
            }
            else result.Add(region.Clone());
            return result;
        }

        /******************************************/
        

        private static bool IsClockwise(this Polyline curve, Vector normal)
        {
            double angleTot = 0;

            List<Vector> dirs = new List<Vector>();
            for (int i = 0; i < curve.ControlPoints.Count - 1; i++)
            {
                dirs.Add(curve.ControlPoints[i + 1] - curve.ControlPoints[i]);
            }

            for (int i = 0; i < dirs.Count; i++)
            {
                angleTot += dirs[i].SignedAngle( dirs[(i + 1) % dirs.Count], normal);
            }
            return angleTot > 0;
        }
    }
}
