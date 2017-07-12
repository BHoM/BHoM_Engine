using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static class VectorUtils
    {
        /// <summary>
        /// Calculate the angle in radians between two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double VectorAngle(Vector a, Vector b)
        {
            return ArrayUtils.Angle(a, b);
        }

        /// <summary>
        /// Computes Acos with tolerance for rounding errors
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double SafeAcos(double d)
        {
            double tol = 0.00001;
            if (d > 1.0)
                if (d - 1.0 <= tol)
                    return Math.Acos(1.0);
            return Math.Acos(d);
        }

        /// <summary>
        /// Calculate the angle in radians between two vectors with a guide normal vector to determine sign
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static double VectorAngle(Vector a, Vector b, Vector normal)
        {
            double angle = VectorAngle(a, b);

            Vector crossproduct = Vector.CrossProduct(a, b);
            if (VectorAngle(crossproduct, normal) < (Math.PI / 2.0))
                return angle;
            else
                return -1.0 * angle;
        }
    }

}
