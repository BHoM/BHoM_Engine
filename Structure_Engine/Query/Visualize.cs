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

        public static List<ICurve> Visualize(this AreaTemperatureLoad areaTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true, bool edgeDisplay = true, bool gridDisplay = false)
        {
            List<ICurve> arrows = new List<ICurve>();
            double loadFactor = areaTempLoad.TemperatureChange * 1000 * scaleFactor; //Arrow methods are scaling down force to 1/1000

            foreach (IAreaElement element in areaTempLoad.Objects.Elements)
            {
                Vector vector = element.INormal() * loadFactor;
                if (edgeDisplay) arrows.AddRange(ConnectedArrows(element.IEdges(), vector, true, null, 0, true));
                if (gridDisplay) arrows.AddRange(MultipleArrows(element.IPointGrid(), vector, true, null, 0, true));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this AreaUniformalyDistributedLoad areaUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true, bool edgeDisplay = true, bool gridDisplay = false)
        {
            if (!displayForces)
                return new List<ICurve>();

            List<ICurve> arrows = new List<ICurve>();
            Vector globalForceVec = areaUDL.Pressure * scaleFactor;

            foreach (IAreaElement element in areaUDL.Objects.Elements)
            {
                Vector forceVec;
                CoordinateSystem system = null;

                IEnumerable<ICurve> edges = element.IEdges();

                if (areaUDL.Axis == LoadAxis.Global)
                {
                    if (areaUDL.Projected)
                    {
                        Vector normal = element.INormal().Normalise();
                        double scale = Math.Abs(normal.DotProduct(globalForceVec.Normalise()));
                        forceVec = globalForceVec * scale;
                    }
                    else
                    {
                        forceVec = globalForceVec;
                    }
                }
                else
                {
                    Vector normal = element.INormal();
                    Vector x = edges.First().IStartDir();
                    Vector y = normal.CrossProduct(x);
                    system = new CoordinateSystem { Z = normal, X = x, Y = y };
                    forceVec = globalForceVec;
                }

                if (edgeDisplay) arrows.AddRange(ConnectedArrows(edges, forceVec, asResultants, system, 1, true));
                if (gridDisplay) arrows.AddRange(MultipleArrows(element.IPointGrid(), forceVec, asResultants, system, 1, true));

            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarPointLoad barPointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barPointForce.Force * scaleFactor;
            Vector momentVec = barPointForce.Moment * scaleFactor;

            foreach (Bar bar in barPointForce.Objects.Elements)
            {
                Point point = bar.StartNode.Position;
                Vector tan = (bar.EndNode.Position - bar.StartNode.Position).Normalise();
                point += tan * barPointForce.DistanceFromA;

                if (displayForces) arrows.AddRange(Arrows(point, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(point, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarPrestressLoad barPrestressLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            foreach (Bar bar in barPrestressLoad.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal()*barPrestressLoad.Prestress, false, null, 0, true));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarTemperatureLoad barTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();
            double loadFactor = barTempLoad.TemperatureChange * 1000 * scaleFactor; //Arrow methods are scaling down force to 1/1000


            foreach (Bar bar in barTempLoad.Objects.Elements)
            {

                if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal() * loadFactor, false, null, 0, true));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarUniformlyDistributedLoad barUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barUDL.Force * scaleFactor;
            Vector momentVec = barUDL.Moment * scaleFactor;

            double sqTol = Tolerance.Distance * Tolerance.Distance;

            foreach (Bar bar in barUDL.Objects.Elements)
            {
                CoordinateSystem system;

                Vector[] forceVectors = BarForceVectors(bar, forceVec, momentVec, barUDL.Axis, barUDL.Projected, out system);

                if (displayForces && forceVectors[0].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[0], asResultants, system, 1, true));
                if (displayMoments && forceVectors[1].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[1], asResultants, system, 1, false));
            }
            

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this BarVaryingDistributedLoad barVaryingDistLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
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

                CoordinateSystem system;

                Vector[] forcesA = BarForceVectors(bar, forceA, momentA, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected, out system);
                Vector[] forcesB = BarForceVectors(bar, forceB, momentB, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected, out system);

                if (displayForces && (forcesA[0].SquareLength() > sqTol || forcesB[0].SquareLength() > sqTol))
                {
                    Point[] prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point[] basePt;
                        Vector v = (1 - factor) * forcesA[0] + factor * forcesB[0];
                        arrows.AddRange(Arrows(pts[i], v, true, asResultants, out basePt, system, 1));

                        if (i > 0)
                        {
                            for (int j = 0; j < basePt.Length; j++)
                            {
                                arrows.Add(new Line { Start = prevPt[j], End = basePt[j] });
                            }

                        }
                        prevPt = basePt;
                    }
                }
                if (displayMoments && (forcesA[1].SquareLength() > sqTol || forcesB[1].SquareLength() > sqTol))
                {
                    Point[] prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point[] basePt;
                        Vector v = (1 - factor) * forcesA[1] + factor * forcesB[1];
                        arrows.AddRange(Arrows(pts[i], v, true, asResultants, out basePt, system, 1));

                        if (i > 0)
                        {
                            for (int j = 0; j < basePt.Length; j++)
                            {
                                arrows.Add(new Line { Start = prevPt[j], End = basePt[j] });
                            }

                        }
                        prevPt = basePt;
                    }
                }
            }


            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this GravityLoad gravityLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
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

                    if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, loadVector, true, null, 1, true));
                }
                else if (obj is IAreaElement)
                {
                    IAreaElement element = obj as IAreaElement;

                    Vector loadVector = element.Property.IMassPerArea() * gravityDir;

                    if (displayForces) arrows.AddRange(ConnectedArrows(element.IEdges(), loadVector, true));
                }
                else
                {
                    Reflection.Compute.RecordWarning("Display for gravity loads only implemented for Bars and IAreaElements. No area elements will be displayed");
                }
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointAcceleration pointAcceleration, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointAcceleration.TranslationalAcceleration * scaleFactor;
            Vector momentVec = pointAcceleration.RotationalAcceleration * scaleFactor;

            foreach (Node node in pointAcceleration.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointDisplacement pointDisplacement, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointDisplacement.Translation * scaleFactor;
            Vector momentVec = pointDisplacement.Rotation * scaleFactor;

            foreach (Node node in pointDisplacement.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointForce pointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointForce.Force * scaleFactor;
            Vector momentVec = pointForce.Moment * scaleFactor;

            foreach (Node node in pointForce.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        public static List<ICurve> Visualize(this PointVelocity pointVelocity, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointVelocity.TranslationalVelocity * scaleFactor;
            Vector momentVec = pointVelocity.RotationalVelocity * scaleFactor;

            foreach (Node node in pointVelocity.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        public static IEnumerable<IGeometry> IVisualize(this ILoad load, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            return Visualize(load as dynamic, scaleFactor, displayForces, displayMoments, asResultants);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Vector[] BarForceVectors(Bar bar, Vector globalForce, Vector globalMoment, LoadAxis axis, bool isProjected, out CoordinateSystem system)
        {
            if (axis == LoadAxis.Global)
            {
                system = null;
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

                system = new CoordinateSystem() { X = tanUnit, Y = y, Z = normal };

                return new Vector[] { globalForce, globalMoment };
            }
        }

        /***************************************************/

        private static List<ICurve> Arrows(Point pt, Vector load, bool straightArrow, bool asResultant, CoordinateSystem coordinateSystem = null, int nbArrowHeads = 1)
        {
            Point[] basePoints;
            return Arrows(pt, load, straightArrow, asResultant, out basePoints, coordinateSystem, nbArrowHeads);
        }

        /***************************************************/

        private static List<ICurve> Arrows(Point pt, Vector load, bool straightArrow, bool asResultant, out Point[] basePoints, CoordinateSystem coordinateSystem = null, int nbArrowHeads = 1)
        {
            if (asResultant)
            {
                Vector vector;
                if (coordinateSystem == null)
                    vector = load;
                else
                    vector = coordinateSystem.X * load.X + coordinateSystem.Y * load.Y + coordinateSystem.Z * load.Z;

                basePoints = new Point[1];
                if (straightArrow)
                    return Arrow(pt, vector, out basePoints[0], nbArrowHeads);
                else
                    return ArcArrow(pt, vector, out basePoints[0]);
            }
            else
            {
                List<ICurve> arrows = new List<ICurve>();
                basePoints = new Point[3];
                Vector[] vectors;

                if (coordinateSystem == null)
                    vectors = new Vector[] { new Vector { X = load.X }, new Vector { Y = load.Y }, new Vector { Z = load.Z } };
                else
                    vectors = new Vector[] { coordinateSystem.X * load.X, coordinateSystem.Y * load.Y, coordinateSystem.Z * load.Z };

                for (int i = 0; i < 3; i++)
                {
                    if (straightArrow)
                        arrows.AddRange(Arrow(pt, vectors[i], out basePoints[i], nbArrowHeads, 0.02));
                    else
                        arrows.AddRange(ArcArrow(pt, vectors[i], out basePoints[i]));
                }
                return arrows;
            }
        }

        /***************************************************/

        private static List<ICurve> Arrow(Point pt, Vector v, int nbArrowHeads = 1, double offsetRatio = 0.0)
        {
            Point basePt;
            return Arrow(pt, v, out basePt, nbArrowHeads, offsetRatio);
        }
        /***************************************************/

        private static List<ICurve> Arrow(Point pt, Vector v, out Point basePt,int nbArrowHeads = 1, double offsetRatio = 0.0)
        {
            List<ICurve> arrow = new List<ICurve>();

            //scale from N to kN and flip to get correct arrows
            v /= -1000;

            pt = pt + v * offsetRatio;
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

            double pi4over3 = Math.PI * 4 / 3;
            Arc arc = Engine.Geometry.Create.Arc(Engine.Geometry.Create.CoordinateSystem(pt, xAxis, yAxis), length / pi4over3, 0, pi4over3);

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

        private static List<ICurve> ConnectedArrows(IEnumerable<ICurve> curves, Vector vector, bool asResultant, CoordinateSystem coordinateSystem = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            Vector[] baseVec;

            int divisions = straightArrow ? 5 : 7;

            allCurves = MultipleArrows(curves.SelectMany(x => x.SamplePoints((int)divisions)), vector, asResultant, out baseVec, coordinateSystem, nbArrowHeads, straightArrow);
            if(straightArrow) allCurves.AddRange(curves.SelectMany(x => baseVec.Select(v => x.ITranslate(v))));

            return allCurves;
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, bool asResultant, CoordinateSystem coordinateSYstem = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            Vector[] baseVec;
            return MultipleArrows(basePoints, vector, asResultant, out baseVec, coordinateSYstem, nbArrowHeads, straightArrow);
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, bool asResultant, out Vector[] baseVec, CoordinateSystem coordinateSystem = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            List<ICurve> arrow = new List<ICurve>();
            Point[] basePts;

            arrow = Arrows(Point.Origin, vector, straightArrow, asResultant, out basePts, coordinateSystem, nbArrowHeads);

            baseVec = basePts.Select(x => x - Point.Origin).ToArray();

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

    }

}
