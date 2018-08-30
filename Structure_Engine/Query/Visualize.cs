using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;

using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static List<Line> Visualize(this AreaTemperatureLoad areaTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Line> Visualize(this AreaUniformalyDistributedLoad areaUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            if (!displayForces)
                return new List<Line>();


            List<Line> arrows = new List<Line>();

            Vector globalForceVec = areaUDL.Pressure * scaleFactor;


            if (areaUDL.Axis == LoadAxis.Global)
            {
                if (areaUDL.Projected)
                {
                    foreach (IAreaElement element in areaUDL.Objects.Elements)
                    {
                        Vector normal = element.INormal().Normalise();
                        double scale = Math.Abs(normal.DotProduct(globalForceVec.Normalise()));

                        arrows.AddRange(ConnectedArrows(element.IEdgePoints(), globalForceVec * scale));

                    }
                }
                else
                {
                    foreach (IAreaElement element in areaUDL.Objects.Elements)
                    {
                        Vector normal = element.INormal().Normalise();
                        arrows.AddRange(ConnectedArrows(element.IEdgePoints(), globalForceVec));

                    }
                }
            }
            else
            {
                Vector globalZ = Vector.ZAxis;
                foreach (IAreaElement element in areaUDL.Objects.Elements)
                {
                    Vector normal = element.INormal();
                    double angle = normal.Angle(globalZ);
                    Vector rotAxis = globalZ.CrossProduct(normal);
                    Vector localForceVec = globalForceVec.Rotate(angle, rotAxis);

                    arrows.AddRange(ConnectedArrows(element.IEdgePoints(), localForceVec));

                }
            }
            return arrows;
        }

        /***************************************************/

        public static List<Line> Visualize(this BarPointLoad barPointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = barPointForce.Force * scaleFactor;
            Vector momentVec = barPointForce.Moment * scaleFactor;

            foreach (Bar bar in barPointForce.Objects.Elements)
            {
                Point point = bar.StartNode.Position;
                Vector tan = (bar.EndNode.Position - bar.StartNode.Position).Normalise();
                point += tan * barPointForce.DistanceFromA;

                if (displayForces) arrows.AddRange(Arrow(point, forceVec));
                if (displayMoments) arrows.AddRange(Arrow(point, momentVec, 2));
            }

            return arrows;
        }

        /***************************************************/

        [NotImplemented]
        public static List<Line> Visualize(this BarPrestressLoad barPrestressLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static List<Line> Visualize(this BarTemperatureLoad barTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Line> Visualize(this BarUniformlyDistributedLoad barUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = barUDL.Force * scaleFactor;
            Vector momentVec = barUDL.Moment * scaleFactor;

            int divisions = 5;

            if (barUDL.Axis == LoadAxis.Global)
            {
                if (barUDL.Projected)
                {
                    foreach (Bar bar in barUDL.Objects.Elements)
                    {
                        Point startPos = bar.StartNode.Position;
                        Vector tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;

                        Vector tanUnit = tan.Normalise();
                        Vector forceUnit = forceVec.Normalise();
                        Vector momentUnit = momentVec.Normalise();

                        double scaleFactorForce = (tanUnit - tanUnit.DotProduct(forceUnit)* forceUnit).Length();
                        double scaleFactorMoment = (tanUnit - tanUnit.DotProduct(momentUnit) * momentUnit).Length();

                        List<Point> pts = DistributedPoints(startPos, tan, divisions);

                        if (displayForces) arrows.AddRange(ConnectedArrows(pts, forceVec * scaleFactorForce, 1, false));
                        if (displayMoments) arrows.AddRange(ConnectedArrows(pts, momentVec * scaleFactorMoment, 2, false));
                    }
                }
                else
                {
                    foreach (Bar bar in barUDL.Objects.Elements)
                    {
                        Point startPos = bar.StartNode.Position;
                        Vector tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;

                        List<Point> pts = DistributedPoints(startPos, tan, divisions);

                        if (displayForces) arrows.AddRange(ConnectedArrows(pts, forceVec, 1, false));
                        if (displayMoments) arrows.AddRange(ConnectedArrows(pts, momentVec, 2, false));
                    }

                }

            }
            else
            {
                Vector globalZ = Vector.ZAxis;
                foreach (Bar bar in barUDL.Objects.Elements)
                {
                    Vector normal = bar.Normal();
                    Vector tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;
                    Vector tanUnit = tan.Normalise();
                    Vector y = normal.CrossProduct(tanUnit);

                    Vector localForceVec = tanUnit * forceVec.X + y * forceVec.Y + normal * forceVec.Z;
                    Vector localMomentVec = tanUnit * momentVec.X + y * momentVec.Y + normal * momentVec.Z;

                    Point startPos = bar.StartNode.Position;

                    List<Point> pts = DistributedPoints(startPos, tan, divisions);

                    if (displayForces) arrows.AddRange(ConnectedArrows(pts, localForceVec, 1, false));
                    if (displayMoments) arrows.AddRange(ConnectedArrows(pts, localMomentVec, 2, false));

                }
            }

            return arrows;
        }

        /***************************************************/

        [NotImplemented]
        public static List<Line> Visualize(this BarVaryingDistributedLoad barVaryingDistLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static List<Line> Visualize(this GravityLoad gravityLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Line> Visualize(this PointAcceleration pointAcceleration, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = pointAcceleration.TranslationalAcceleration * scaleFactor;
            Vector momentVec = pointAcceleration.RotationalAcceleration * scaleFactor;

            foreach (Node node in pointAcceleration.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(Arrow(node.Position, momentVec, 2));
            }

            return arrows;
        }

        /***************************************************/

        public static List<Line> Visualize(this PointDisplacement pointDisplacement, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = pointDisplacement.Translation * scaleFactor;
            Vector momentVec = pointDisplacement.Rotation * scaleFactor;

            foreach (Node node in pointDisplacement.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(Arrow(node.Position, momentVec, 2));
            }

            return arrows;
        }

        /***************************************************/

        public static List<Line> Visualize(this PointForce pointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = pointForce.Force * scaleFactor;
            Vector momentVec = pointForce.Moment * scaleFactor;

            foreach (Node node in pointForce.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(Arrow(node.Position, momentVec, 2));
            }

            return arrows;
        }

        /***************************************************/

        public static List<Line> Visualize(this PointVelocity pointVelocity, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = pointVelocity.TranslationalVelocity * scaleFactor;
            Vector momentVec = pointVelocity.RotationalVelocity * scaleFactor;

            foreach (Node node in pointVelocity.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(Arrow(node.Position, momentVec, 2));
            }

            return arrows;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        public static IEnumerable<IGeometry> IVisualize(this ILoad load, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            return Visualize(load as dynamic, scaleFactor);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Line> Arrow(Point pt, Vector v, int nbArrowHeads = 1)
        {
            List<Line> arrow = new List<Line>();

            Point end = pt + v;

            arrow.Add(Engine.Geometry.Create.Line(pt, end));

            double length = v.Length();

            Vector tan = v / length;

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(tan);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
            {
                v1 = Vector.YAxis;
                dot = v1.DotProduct(tan);
            }

            v1 = (v1 - dot * tan).Normalise();

            Vector v2 = v1.CrossProduct(tan).Normalise();

            v1 /= 2;
            v2 /= 2;

            double factor = length / 10;

            int m = 0;

            while (m < nbArrowHeads)
            {
                arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + tan) * factor));

                pt = pt + tan * factor;
                m++;
            }


            return arrow;
        }

        /***************************************************/

        private static List<Line> ConnectedArrows(List<Point> basePoints, Vector vector, int nbArrowHeads = 1, bool loop = true)
        {
            List<Line> allLines = new List<Line>();

            List<Line> arrow = Arrow(Point.Origin, vector);

            Point thisPt = null;
            Point prevPt = null;

            for (int i = 0; i < basePoints.Count; i++)
            {
                Vector vec = basePoints[i] - Point.Origin;
                allLines.AddRange(arrow.Select(x => x.Translate(vec)));

                thisPt = basePoints[i] + vector;

                if (i > 0)
                {
                    allLines.Add(new Line { Start = prevPt, End = thisPt});
                }

                prevPt = thisPt;
            }

            if (loop)
            {
                allLines.Add(new Line { Start = prevPt, End = basePoints[0] + vector });
            }

            return allLines;
        }

        /***************************************************/

        private static List<Point> DistributedPoints(Point basePt, Vector step, int divisions)
        {
            List<Point> pts = new List<Point>();

            for (int i = 0; i <= divisions; i++)
            {
                pts.Add(basePt + step * i);
            }
            return pts;
        }

        /***************************************************/

        private static List<Point> EdgePoints(this PanelPlanar panel)
        {
            List<ICurve> edges = panel.ExternalEdgeCurves();

            return edges.SelectMany(x => x.SamplePoints((int)5)).ToList();
        }

        /***************************************************/

        private static List<Point> EdgePoints(this MeshFace face)
        {
            List<Point> pts = new List<Point>();

            for (int i = 0; i < face.Nodes.Count; i++)
            {
                pts.Add(face.Nodes[i].Position);

                int nextId = i < face.Nodes.Count - 1 ? i + 1 : 0;

                pts.Add((face.Nodes[i].Position + face.Nodes[nextId].Position) / 2);
            }

            return pts;
        }

        /***************************************************/

        private static List<Point> IEdgePoints(this IAreaElement areaElement)
        {
            return EdgePoints(areaElement as dynamic);
        }

        /***************************************************/

    }

}
