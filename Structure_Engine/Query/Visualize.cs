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
            Reflection.Compute.RecordWarning("No visualization implemented for AreaTempratureLoads");
            return new List<Line>();
        }

        /***************************************************/

        public static List<ICurve> Visualize(this AreaUniformalyDistributedLoad areaUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool edgeDisplay = true, bool gridDisplay = false)
        {
            if (!displayForces)
                return new List<ICurve>();

            List<ICurve> arrows = new List<ICurve>();
            Vector globalForceVec = areaUDL.Pressure * scaleFactor;


            if (areaUDL.Axis == LoadAxis.Global)
            {
                if (areaUDL.Projected)
                {
                    foreach (IAreaElement element in areaUDL.Objects.Elements)
                    {
                        Vector normal = element.INormal().Normalise();
                        double scale = Math.Abs(normal.DotProduct(globalForceVec.Normalise()));

                        if (edgeDisplay) arrows.AddRange(ConnectedArrows(element.IEdges(), globalForceVec * scale));
                        if (gridDisplay) arrows.AddRange(MultipleArrows(element.IPointGrid(), globalForceVec * scale));
                    }
                }
                else
                {
                    foreach (IAreaElement element in areaUDL.Objects.Elements)
                    {
                        Vector normal = element.INormal().Normalise();
                        if (edgeDisplay) arrows.AddRange(ConnectedArrows(element.IEdges(), globalForceVec));
                        if (gridDisplay) arrows.AddRange(MultipleArrows(element.IPointGrid(), globalForceVec));

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

                    if (edgeDisplay) arrows.AddRange(ConnectedArrows(element.IEdges(), localForceVec));
                    if (gridDisplay) arrows.AddRange(MultipleArrows(element.IPointGrid(), localForceVec));

                }
            }
            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarPointLoad barPointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barPointForce.Force * scaleFactor;
            Vector momentVec = barPointForce.Moment * scaleFactor;

            foreach (Bar bar in barPointForce.Objects.Elements)
            {
                Point point = bar.StartNode.Position;
                Vector tan = (bar.EndNode.Position - bar.StartNode.Position).Normalise();
                point += tan * barPointForce.DistanceFromA;

                if (displayForces) arrows.AddRange(Arrow(point, forceVec));
                if (displayMoments) arrows.AddRange(ArcArrow(point, momentVec));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarPrestressLoad barPrestressLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            int divisions = 5;

            foreach (Bar bar in barPrestressLoad.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal()*barPrestressLoad.Prestress, 0, true));

            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarTemperatureLoad barTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            int divisions = 5;

            foreach (Bar bar in barTempLoad.Objects.Elements)
            {
                double loadFactor = bar.SectionProperty.Area * bar.SectionProperty.Material.CoeffThermalExpansion * bar.SectionProperty.Material.YoungsModulus * barTempLoad.TemperatureChange;

                if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal() * loadFactor, 0, true));

            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarUniformlyDistributedLoad barUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barUDL.Force * scaleFactor;
            Vector momentVec = barUDL.Moment * scaleFactor;

            int divisions = 5;
            double sqTol = Tolerance.Distance * Tolerance.Distance;

            foreach (Bar bar in barUDL.Objects.Elements)
            {
                List<Point> pts = DistributedPoints(bar, divisions);

                Vector[] forceVectors = BarForceVectors(bar, forceVec, momentVec, barUDL.Axis, barUDL.Projected);

                if (displayForces && forceVectors[0].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[0], 1, true));
                if (displayMoments && forceVectors[1].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[1], 1, false));
            }
            

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarVaryingDistributedLoad barVaryingDistLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceA = barVaryingDistLoad.ForceA * scaleFactor;
            Vector forceB = barVaryingDistLoad.ForceB * scaleFactor;
            Vector momentA = barVaryingDistLoad.MomentA * scaleFactor;
            Vector momentB = barVaryingDistLoad.MomentB * scaleFactor;

            int divisions = 5;
            double sqTol = Tolerance.Distance * Tolerance.Distance;

            foreach (Bar bar in barVaryingDistLoad.Objects.Elements)
            {
                List<Point> pts = DistributedPoints(bar, divisions, barVaryingDistLoad.DistanceFromA, barVaryingDistLoad.DistanceFromB);

                Vector[] forcesA = BarForceVectors(bar, forceA, momentA, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected);
                Vector[] forcesB = BarForceVectors(bar, forceB, momentB, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected);

                if (displayForces && (forcesA[0].SquareLength() > sqTol || forcesB[0].SquareLength() > sqTol))
                {
                    Point prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point basePt;
                        Vector v = (1 - factor) * forcesA[0] + factor * forcesB[0];
                        arrows.AddRange(Arrow(pts[i], v, out basePt, 1));

                        if (i > 0)
                        {
                            arrows.Add(new Line { Start = prevPt, End = basePt });
                        }
                        prevPt = basePt;
                    }
                }
                if (displayMoments && (forcesA[1].SquareLength() > sqTol || forcesB[1].SquareLength() > sqTol))
                {
                    Point prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point basePt;
                        Vector v = (1 - factor) * forcesA[1] + factor * forcesB[1];
                        arrows.AddRange(ArcArrow(pts[i], v, out basePt));

                        if (i > 0)
                        {
                            arrows.Add(new Line { Start = prevPt, End = basePt });
                        }
                        prevPt = basePt;
                    }
                }
            }


            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this GravityLoad gravityLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector gravityDir = gravityLoad.GravityDirection * scaleFactor * 9.80665;
            int barDivisions = 5;

            foreach (BH.oM.Base.BHoMObject obj in gravityLoad.Objects.Elements)
            {
                if (obj is Bar)
                {
                    Bar bar = obj as Bar;

                    if (bar.SectionProperty == null || bar.SectionProperty.Material == null)
                    {
                        Reflection.Compute.RecordWarning("Bar needs a valid sectionproperty and material to display gravity loading");
                        continue;
                    }

                    Vector loadVector = bar.SectionProperty.MassPerMetre() * gravityDir;

                    List<Point> pts = DistributedPoints(bar, barDivisions);

                    if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, loadVector, 1, true));
                }
                else
                {
                    Reflection.Compute.RecordWarning("Display for gravity loads only implemented for Bars. No area elements will be displayed");
                }
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointAcceleration pointAcceleration, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointAcceleration.TranslationalAcceleration * scaleFactor;
            Vector momentVec = pointAcceleration.RotationalAcceleration * scaleFactor;

            foreach (Node node in pointAcceleration.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(ArcArrow(node.Position, momentVec));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointDisplacement pointDisplacement, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointDisplacement.Translation * scaleFactor;
            Vector momentVec = pointDisplacement.Rotation * scaleFactor;

            foreach (Node node in pointDisplacement.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(ArcArrow(node.Position, momentVec));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointForce pointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointForce.Force * scaleFactor;
            Vector momentVec = pointForce.Moment * scaleFactor;

            foreach (Node node in pointForce.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(ArcArrow(node.Position, momentVec));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointVelocity pointVelocity, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointVelocity.TranslationalVelocity * scaleFactor;
            Vector momentVec = pointVelocity.RotationalVelocity * scaleFactor;

            foreach (Node node in pointVelocity.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrow(node.Position, forceVec));
                if (displayMoments) arrows.AddRange(ArcArrow(node.Position, momentVec));
            }

            return arrows;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        public static IEnumerable<IGeometry> IVisualize(this ILoad load, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true)
        {
            return Visualize(load as dynamic, scaleFactor, displayForces, displayMoments);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Vector[] BarForceVectors(Bar bar, Vector globalForce, Vector globalMoment, LoadAxis axis, bool isProjected)
        {
            if (axis == LoadAxis.Global)
            {
                if (isProjected)
                {
                    Point startPos = bar.StartNode.Position;
                    Vector tan = (bar.EndNode.Position - bar.StartNode.Position);

                    Vector tanUnit = tan.Normalise();
                    Vector forceUnit = globalForce.Normalise();
                    Vector momentUnit = globalMoment.Normalise();

                    double scaleFactorForce = (tanUnit - tanUnit.DotProduct(forceUnit) * forceUnit).Length();
                    double scaleFactorMoment = (tanUnit - tanUnit.DotProduct(momentUnit) * momentUnit).Length();

                    return new Vector[] { globalForce * scaleFactorForce, globalMoment * scaleFactorMoment };
                }
                else
                {
                    return new Vector[] { globalForce, globalMoment };
                }
            }
            else
            {

                Vector normal = bar.Normal();
                Vector tan = (bar.EndNode.Position - bar.StartNode.Position);
                Vector tanUnit = tan.Normalise();
                Vector y = normal.CrossProduct(tanUnit);

                Vector localForceVec = tanUnit * globalForce.X + y * globalForce.Y + normal * globalForce.Z;
                Vector localMomentVec = tanUnit * globalMoment.X + y * globalMoment.Y + normal * globalMoment.Z;

                return new Vector[] { localForceVec, localMomentVec };
            }
        }

        /***************************************************/

        private static List<Line> Arrow(Point pt, Vector v, int nbArrowHeads = 1)
        {
            Point basePt;
            return Arrow(pt, v, out basePt, nbArrowHeads);
        }
        /***************************************************/

        private static List<Line> Arrow(Point pt, Vector v, out Point basePt,int nbArrowHeads = 1)
        {
            List<Line> arrow = new List<Line>();

            //scale from N to kN and flip to get correct arrows
            v /= -1000;

            Point end = pt + v;

            arrow.Add(Engine.Geometry.Create.Line(pt, end));

            double length = v.Length();

            Vector tan = v / length;

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(tan);

            if (Math.Abs(1 - Math.Abs(dot)) < Tolerance.Angle)
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

            basePt = end;

            return arrow;
        }

        /***************************************************/

        private static List<ICurve> ArcArrow(Point pt, Vector v)
        {
            Point startPt;
            return ArcArrow(pt, v, out startPt);
        }

        /***************************************************/

        private static List<ICurve> ArcArrow(Point pt, Vector v, out Point startPt)
        {
            List<ICurve> arrow = new List<ICurve>();

            //Scale from Nm to kNm
            v = v / 1000;

            double length = v.Length();

            Vector cross;
            if (v.IsParallel(Vector.ZAxis) == 0)
                cross = Vector.ZAxis;
            else
                cross = Vector.YAxis;

            Vector yAxis = v.CrossProduct(cross);
            Vector xAxis = yAxis.CrossProduct(v);

            Arc arc = Engine.Geometry.Create.Arc(Engine.Geometry.Create.CoordinateSystem(pt, xAxis, yAxis), length / (Math.PI * 2), 0, Math.PI * 4 / 3);

            startPt = arc.StartPoint();

            arrow.Add(arc);

            Vector tan = -arc.EndDir();

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

            pt = arc.EndPoint();

            arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + tan) * factor));


            return arrow;
        }

        /***************************************************/

        private static List<ICurve> ConnectedArrows(IEnumerable<ICurve> curves, Vector vector, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            Vector baseVec;

            allCurves = MultipleArrows(curves.SelectMany(x => x.SamplePoints((int)5)), vector, out baseVec, nbArrowHeads, straightArrow);
            allCurves.AddRange(curves.Select(x => x.ITranslate(baseVec)));

            return allCurves;
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, int nbArrowHeads = 1, bool straightArrow = true)
        {
            Vector baseVec;
            return MultipleArrows(basePoints, vector, out baseVec, nbArrowHeads, straightArrow);
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, out Vector baseVec, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            List<ICurve> arrow = new List<ICurve>();
            Point basePt;
            if (straightArrow)
                arrow.AddRange(Arrow(Point.Origin, vector, out basePt, nbArrowHeads));
            else
                arrow = ArcArrow(Point.Origin, vector, out basePt);

            baseVec = basePt - Point.Origin;

            foreach (Point pt in basePoints)
            {
                Vector vec = pt - Point.Origin;
                allCurves.AddRange(arrow.Select(x => x.ITranslate(vec)));
            }

            return allCurves;
        }

        /***************************************************/

        private static List<Point> DistributedPoints(Bar bar, int divisions, double startLength = 0, double endLength = 0)
        {
            Point startPos;
            Vector tan;
            if (startLength == 0 && endLength == 0)
            {
                startPos = bar.StartNode.Position;
                tan = (bar.EndNode.Position - bar.StartNode.Position) / (double)divisions;
            }
            else
            {
                double length = bar.Length();
                tan = (bar.EndNode.Position - bar.StartNode.Position) / length;
                startPos = bar.StartNode.Position + tan * startLength;

                tan *= (length - endLength - startLength) / (double)divisions;
            }

            List<Point> pts = new List<Point>();

            for (int i = 0; i <= divisions; i++)
            {
                pts.Add(startPos + tan * i);
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
