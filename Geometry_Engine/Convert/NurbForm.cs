using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods - Curves                  ****/
        /***************************************************/

        public static NurbCurve ToNurbCurve(this Arc arc)
        {
            //double[] centre = arc.Centre;
            //double[] P1 = CollectionUtils.SubArray<double>(arc.ControlPointVector, 0, arc.Dimensions + 1);
            //double[] P2 = CollectionUtils.SubArray<double>(arc.ControlPointVector, (arc.Dimensions + 1) * 2, arc.Dimensions + 1);

            //double[] V1 = ArrayUtils.Sub(P1, centre);
            //double[] V2 = ArrayUtils.Sub(P2, centre);

            //double[] Normal = ArrayUtils.CrossProduct(V1, V2);

            //double[] T1 = ArrayUtils.CrossProduct(V1, Normal);
            //double[] T2 = ArrayUtils.CrossProduct(V2, Normal);

            //double[] cP2 = ArrayUtils.Intersect(P1, T1, P2, T2);

            //double w2 = arc.Radius() / ArrayUtils.Length(ArrayUtils.Sub(cP2, centre));

            //double arcAngle = ArrayUtils.Angle(V1, V2);

            //arc.SetWeights(new double[] { 1, w2, 1 });
            //arc.SetKnots(new double[] { 0, 0, 0, arcAngle, arcAngle, arcAngle });
            //arc.SetDegree(2);

            //Array.Copy(cP2, 0, arc.ControlPointVector, arc.Dimensions + 1, arc.Dimensions + 1);

            throw new NotImplementedException();
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Circle circle)
        {
            //double root2on2 = Math.Sqrt(2) / 2;
            //double radius = circle.Radius;
            //circle.SetKnots(new double[] { 0, 0, 0, Math.PI / 2, Math.PI / 2, Math.PI, Math.PI, 3 * Math.PI / 2, 3 * Math.PI / 2, 2 * Math.PI, 2 * Math.PI, 2 * Math.PI });
            //circle.SetWeights(new double[] { 1, root2on2, 1, root2on2, 1, root2on2, 1, root2on2, 1 });
            //circle.SetControlPoints(new double[]
            //{
            //    radius, 0, 0, 1,
            //    radius, radius, 0, 1,
            //    0, radius, 0, 1,
            //   -radius, radius, 0, 1,
            //   -radius, 0, 0, 1,
            //   -radius,-radius, 0, 1,
            //    0,-radius, 0, 1,
            //    radius,-radius, 0, 1,
            //    radius, 0, 0, 1
            //});

            //if (circle.Plane.Normal.Z < 1)
            //{
            //    Vector axis = new Vector(ArrayUtils.CrossProduct(circle.Plane.Normal, new double[] { 0, 0, 1, 0 }));
            //    double angle = ArrayUtils.Angle(circle.Plane.Normal, new double[] { 0, 0, 1, 0 });
            //    Transform t = Transform.Rotation(Point.Origin, axis, angle);
            //    t = Transform.Translation(circle.Plane.Origin - Point.Origin) * t;
            //    circle.SetControlPoints(ArrayUtils.MultiplyMany(t, circle.ControlPointVector));
            //}
            //else
            //{
            //    Transform t = Transform.Translation(circle.Plane.Origin - Point.Origin);
            //    circle.SetControlPoints(ArrayUtils.MultiplyMany(t, circle.ControlPointVector));
            //}

            throw new NotImplementedException();
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Line line)
        {
            return new NurbCurve(new List<Point> { line.Start, line.End }, new double[] { 1, 1 }, new double[] { 0, 0, 1, 1 });

        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this NurbCurve curve)
        {
            return curve.GetClone();
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this PolyCurve curve)
        {
            //Curve c = curve.Curves[0];
            //for (int i = 1; i < curve.Curves.Count; i++)
            //{
            //    c.Append(curve.Curves[i]);
            //}
            //curve.SetControlPoints(c.ControlPointVector);
            //curve.SetKnots(c.Knots);
            //curve.SetWeights(c.Weights);
            //curve.SetDegree(c.Degree);

            throw new NotImplementedException();
        }

        /***************************************************/

        public static NurbCurve ToNurbCurve(this Polyline curve)
        {
            //line.SetDegree(1);
            //line.SetKnots(new double[line.ControlPointVector.Length / (line.Dimensions + 1) + line.Order]);
            //line.SetWeights(new double[line.ControlPointVector.Length / (line.Dimensions + 1)]);
            //line.Knots[0] = 0;
            //line.Knots[line.Knots.Length - 1] = line.Weights.Length - 1;
            //for (int i = 0; i < line.Weights.Length; i++)
            //{
            //    line.Knots[i + 1] = i;
            //    line.Weights[i] = 1;
            //}

            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interaces                ****/
        /***************************************************/

        public static NurbCurve IToNurbCurve(this ICurve geometry)
        {
            return ToNurbCurve(geometry as dynamic);
        }

    }
}
