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
    }
}
