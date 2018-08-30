using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;

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

        public static List<Line> Visualize(this BarUniformlyDistributedLoad barUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<Line> arrows = new List<Line>();

            Vector forceVec = barUDL.Force * scaleFactor;
            Vector momentVec = barUDL.Moment * scaleFactor;

            int divisions = 5;

            foreach (Bar bar in barUDL.Objects.Elements)
            {
                Point startPos = bar.StartNode.Position;
                Vector tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;

                for (int i = 0; i <= divisions; i++)
                {
                    if (displayForces) arrows.AddRange(Arrow(startPos + tan * i, forceVec));
                    if (displayMoments) arrows.AddRange(Arrow(startPos + tan * i, momentVec, 2));
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
                        double scale = normal.DotProduct(globalForceVec.Normalise());

                        List<Line> arrow = Arrow(Point.Origin, globalForceVec * scale);

                        foreach (Point pt in element.IPointGrid())
                        {
                            Vector vec = pt - Point.Origin;
                            arrows.AddRange(arrow.Select(x => x.Translate(vec)));
                        }

                    }
                }
                else
                {
                    List<Line> arrow = Arrow(Point.Origin, globalForceVec);
                    List<Point> pts = areaUDL.Objects.Elements.SelectMany(x => x.IPointGrid()).ToList();

                    foreach (Point pt in pts)
                    {
                        Vector vec = pt - Point.Origin;
                        arrows.AddRange(arrow.Select(x => x.Translate(vec)));
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

                    List<Line> arrow = Arrow(Point.Origin, localForceVec);

                    foreach (Point pt in element.IPointGrid())
                    {
                        Vector vec = pt - Point.Origin;
                        arrows.AddRange(arrow.Select(x => x.Translate(vec)));
                    }

                }
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

            double factor = length / 10;

            int m = 0;

            while (m < nbArrowHeads)
            {
                arrow.Add(Engine.Geometry.Create.Line(end, (v1 - tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(end, (-v1 - tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(end, (v2 - tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(end, (-v2 - tan) * factor));

                end = end - tan * factor;
                m++;
            }


            return arrow;
        }

        /***************************************************/

    }

}
