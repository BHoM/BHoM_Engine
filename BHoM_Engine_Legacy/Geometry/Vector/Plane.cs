
using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static class XPlane
    {       
        internal static bool IsSameSide(this Plane plane, double[] p1, double[] p2, double tolerance)
        {
            double dotProduct1 = ArrayUtils.DotProduct(p1, plane.Normal, p1.Length)[0];
            double dotProduct2 = ArrayUtils.DotProduct(p2, plane.Normal, p2.Length)[0];

            if (dotProduct1 + plane.D >= -tolerance && dotProduct2 + plane.D >= -tolerance)
                return true;
            else if (dotProduct1 + plane.D < tolerance && dotProduct2 + plane.D < tolerance)
                return true;
            return false;
        }

        internal static bool IsSameSide(this Plane plane, double[] pnts, double tolerance)
        {
            int[] side = plane.GetSide(pnts, tolerance);

            int nonZeroIndex = 0;
            while (nonZeroIndex < side.Length && side[nonZeroIndex] == 0) nonZeroIndex++;

            int previousSide = side[nonZeroIndex];

            for (int i = nonZeroIndex; i < side.Length; i++)
            {
                if (side[i] != 0 && side[i] != previousSide)
                {
                    return false;
                }
            }
            return true;
        }

        internal static int[] GetSide(this Plane plane, double[] pnts, double tolerance)
        {
            double[] result = ArrayUtils.DotProduct(pnts, plane.Normal, 4);
            int[] sameSide = new int[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] + plane.D > tolerance)
                {
                    sameSide[i] = 1;
                }
                else if (result[i] + plane.D < -tolerance)
                {
                    sameSide[i] = -1;
                }
                else
                {
                    sameSide[i] = 0;
                }
            }
            return sameSide;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double DistanceTo(this Plane plane, Point point)
        {
            return (ArrayUtils.DotProduct(plane.Normal, point) + plane.D) / ArrayUtils.Length(plane.Normal);
        }


        internal static double[] ProjectionVectors(this Plane plane, double[] v, double multiplier = 1)
        {
            double[] normal = plane.Normal;
            double[] distances = ArrayUtils.Sum(ArrayUtils.Multiply(v, normal), 4);
            double[] vectors = new double[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                vectors[i] = normal[i % 4] * -(distances[i / 4] + plane.D) * multiplier;
            }
            return vectors;
        }

        internal static double[] ProjectionVectors(this Plane plane, double[] pnts, double[] direction)
        {
            double[] normal = plane.Normal;
            double[] distances = ArrayUtils.Sum(ArrayUtils.Multiply(pnts, normal), 3);
            double[] vectors = new double[pnts.Length];
            double angle = ArrayUtils.Angle(normal, direction);
            double cosAngle = Math.Cos(angle);

            for (int i = 0; i < pnts.Length; i++)
            {
                vectors[i] = direction[i % 4] * -(distances[i / 4] + plane.D) / cosAngle;
            }
            return vectors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool InPlane(this Plane plane, Point p, double tolerance)
        {
            double dotProduct = ArrayUtils.DotProduct(plane.Normal, p) + plane.D;
            return dotProduct < tolerance && dotProduct > -tolerance;
        }


        internal static bool InPlane(this Plane plane, double[] pnts, int length, double tolerance)
        {
            double[] dotProducts = ArrayUtils.DotProduct(pnts, plane.Normal, length);
            double sum = ArrayUtils.Sum(dotProducts);

            for (int i = 0; i < dotProducts.Length; i++)
            {
                if (dotProducts[i] + plane.D > tolerance || dotProducts[i] + plane.D < -tolerance) return false;
            }

            return true;
        }


        public static void Transform(Plane plane, Transform t)
        {
            plane.Normal.Transform(t);
            plane.Origin.Transform(t);
            plane.Update();
        }

        public static void Translate(Plane plane, Vector v)
        {
            plane.Origin.Translate(v);
            plane.Update();
        }

        /// <summary>
        /// Mirrors vector about a plane
        /// </summary>
        /// <param name="p"></param>
        public static void Mirror(Plane plane, Plane p)
        {
            plane.Normal.Mirror(p);
            plane.Origin.Mirror(p);
            plane.Update();
        }

        /// <summary>
        /// Projects a vector onto a plane
        /// </summary>
        /// <param name="plane"></param>
        public static void Project(Plane plane, Plane p)
        {
            plane.Origin.Project(p);
            plane.Normal = p.Normal;
        }
    }
}
