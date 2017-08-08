using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{

   
    public static class XVector
    {
        ///// <summary>
        ///// Rotates vector using Rodrigues' rotation formula
        ///// </summary>
        ///// <param name="rad"></param>
        ///// <param name="axis"></param>
        ///// <returns></returns>
        //public static Vector Rotate(this Vector vector, double rad, Vector axis)
        //{
        //    // using Rodrigues' rotation formula
        //    axis = axis.Normalise();

        //    return vector * Math.Cos(rad) + Vector.CrossProduct(axis, vector) * Math.Sin(rad) + axis * (axis * vector) * (1 - Math.Cos(rad));
        //}

       //public static double AngleTo(this Vector v1, Vector v2)
       // {
       //     return VectorUtils.VectorAngle(v1, v2);
       // }

        //public static bool IsParallel(this Vector vector, Vector other, double tolerance = 0.0001)
        //{
        //    return (ArrayUtils.Parallel(vector.Coordinates, other.Coordinates, tolerance) != 0);
        //}

        //public static void Transform(Vector point, Transform t)
        //{
        //    point.SetCoordinates(ArrayUtils.Multiply(t, point.Coordinates));
        //}

        //public static void Translate(Vector point, Vector v)
        //{
        //}

        //public static void Mirror(Vector point, Plane p)
        //{
        //    point.SetCoordinates(ArrayUtils.Add(p.ProjectionVectors(point.Coordinates, 2), point.Coordinates));
        //}

        //public static void Project(Vector point, Plane p)
        //{
        //    point.SetCoordinates(ArrayUtils.Add(p.ProjectionVectors(point.Coordinates), point.Coordinates));
        //}
    }
}