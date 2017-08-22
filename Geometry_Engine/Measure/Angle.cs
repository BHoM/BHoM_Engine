using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetAngle(this Vector v1, Vector v2)
        {
            double dotProduct = v1.GetDotProduct(v2);
            double length = v1.GetLength() * v2.GetLength();

            return (Math.Abs(dotProduct) < length) ? Math.Acos(dotProduct / length) : (Math.Abs(dotProduct) < length + 0.0001) ? Math.PI : 0;
        }

        /***************************************************/

        public static double GetSignedAngle(this Vector a, Vector b, Vector normal) // use normal vector to define the sign of the angle
        {
            double angle = GetAngle(a, b);

            Vector crossproduct =a.GetCrossProduct(b);
            if (crossproduct.GetDotProduct(normal) < 0)
                return -angle;
            else
                return angle;
        }

        /***************************************************/

        public static double GetAngle(this Arc arc)
        {
            Point centre = arc.GetCentre();
            return GetAngle(arc.Start-centre, arc.End-centre);
        }
    }
}
